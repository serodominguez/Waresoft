import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import axios, { AxiosError } from 'axios'

// ─── vi.hoisted: variables disponibles ANTES del hoist de vi.mock ─────────────
const { mockLogout, mockHandle, mockHandleSilent, mockRouter } = vi.hoisted(() => {
  const mockLogout = vi.fn()
  const mockHandle = vi.fn()
  const mockHandleSilent = vi.fn()
  const mockRouter = {
    currentRoute: { value: { name: 'dashboard' as string | symbol } },
  }
  return { mockLogout, mockHandle, mockHandleSilent, mockRouter }
})

// ─── Mocks ────────────────────────────────────────────────────────────────────
vi.mock('@/stores/authStore', () => ({
  useAuthStore: () => ({ logout: mockLogout }),
}))

vi.mock('@/router', () => ({
  default: mockRouter,
}))

vi.mock('@/helpers/errorHandler', () => ({
  ErrorHandler: {
    handle: mockHandle,
    handleSilent: mockHandleSilent,
  },
}))

vi.mock('jwt-decode', () => ({
  jwtDecode: vi.fn(),
}))

// ─── Imports bajo test (después de los mocks) ─────────────────────────────────
import { jwtDecode } from 'jwt-decode'
import { setupAxiosInterceptors, configureAxiosDefaults } from '@/plugins/axiosInterceptor'

// ─── Helpers ──────────────────────────────────────────────────────────────────
function makeAxiosError(status?: number, code?: string, message?: string): AxiosError {
  const err = new AxiosError(message ?? 'error', code)
  if (status !== undefined) {
    err.response = { status, data: {}, headers: {}, config: {} as any, statusText: '' }
  }
  return err
}

async function runRequestInterceptor(config: any) {
  const handler = (axios.interceptors.request as any).handlers.at(-1)
  return handler.fulfilled(config)
}

async function runResponseErrorInterceptor(error: any) {
  const handler = (axios.interceptors.response as any).handlers.at(-1)
  return handler.rejected(error)
}

// ─── Setup ────────────────────────────────────────────────────────────────────
beforeEach(() => {
  axios.interceptors.request.clear()
  axios.interceptors.response.clear()

  mockRouter.currentRoute.value.name = 'dashboard'
  mockLogout.mockReset()
  mockHandle.mockReset()
  mockHandleSilent.mockReset()
  ;(jwtDecode as ReturnType<typeof vi.fn>).mockReset()

  localStorage.clear()
  setupAxiosInterceptors()
})

afterEach(() => {
  axios.interceptors.request.clear()
  axios.interceptors.response.clear()
})

// ─────────────────────────────────────────────────────────────────────────────
describe('isTokenExpired (a través del request interceptor)', () => {
  it('considera expirado si jwtDecode lanza una excepción', async () => {
    ;(jwtDecode as ReturnType<typeof vi.fn>).mockImplementation(() => { throw new Error('bad token') })
    localStorage.setItem('token', 'token-malformado')

    await expect(runRequestInterceptor({ headers: {} }))
      .rejects.toMatchObject({ isTokenExpired: true })
  })

  it('considera expirado si exp < Date.now() / 1000', async () => {
    ;(jwtDecode as ReturnType<typeof vi.fn>).mockReturnValue({ exp: 1 })
    localStorage.setItem('token', 'token-expirado')

    await expect(runRequestInterceptor({ headers: {} }))
      .rejects.toMatchObject({ isTokenExpired: true })
  })

  it('considera válido si exp > Date.now() / 1000', async () => {
    const futureExp = Math.floor(Date.now() / 1000) + 3600
    ;(jwtDecode as ReturnType<typeof vi.fn>).mockReturnValue({ exp: futureExp })
    localStorage.setItem('token', 'token-valido')

    const result = await runRequestInterceptor({ headers: {} as any })

    expect(result.headers.Authorization).toBe('Bearer token-valido')
  })
})

// ─────────────────────────────────────────────────────────────────────────────
describe('Request interceptor', () => {
  it('no agrega Authorization si no hay token en localStorage', async () => {
    const result = await runRequestInterceptor({ headers: {} as any })

    expect(result.headers.Authorization).toBeUndefined()
  })

  it('cancela la petición con isTokenExpired:true si el token expiró', async () => {
    ;(jwtDecode as ReturnType<typeof vi.fn>).mockReturnValue({ exp: 1 })
    localStorage.setItem('token', 'expirado')

    await expect(runRequestInterceptor({ headers: {} }))
      .rejects.toMatchObject({ message: 'Token expirado', isTokenExpired: true })
  })

  it('agrega Authorization header con token válido', async () => {
    const futureExp = Math.floor(Date.now() / 1000) + 3600
    ;(jwtDecode as ReturnType<typeof vi.fn>).mockReturnValue({ exp: futureExp })
    localStorage.setItem('token', 'mi-token')

    const result = await runRequestInterceptor({ headers: {} as any })

    expect(result.headers.Authorization).toBe('Bearer mi-token')
  })

  it('agrega Content-Type: application/json si el body NO es FormData', async () => {
    const futureExp = Math.floor(Date.now() / 1000) + 3600
    ;(jwtDecode as ReturnType<typeof vi.fn>).mockReturnValue({ exp: futureExp })
    localStorage.setItem('token', 'mi-token')

    const result = await runRequestInterceptor({ headers: {} as any, data: { foo: 'bar' } })

    expect(result.headers['Content-Type']).toBe('application/json')
  })

  it('NO agrega Content-Type si el body es FormData', async () => {
    const futureExp = Math.floor(Date.now() / 1000) + 3600
    ;(jwtDecode as ReturnType<typeof vi.fn>).mockReturnValue({ exp: futureExp })
    localStorage.setItem('token', 'mi-token')

    const result = await runRequestInterceptor({ headers: {} as any, data: new FormData() })

    expect(result.headers['Content-Type']).toBeUndefined()
  })
})

// ─────────────────────────────────────────────────────────────────────────────
describe('Response interceptor — token expirado (isTokenExpired)', () => {
  it('muestra toast y llama logout cuando la ruta actual NO es login', async () => {
    mockRouter.currentRoute.value.name = 'dashboard'
    const error = { isTokenExpired: true, message: 'Token expirado' }

    await expect(runResponseErrorInterceptor(error))
      .rejects.toMatchObject({ isTokenExpired: true })

    expect(mockHandle).toHaveBeenCalledWith(error, expect.objectContaining({ showToast: true }))
    expect(mockLogout).toHaveBeenCalledOnce()
  })

  it('NO llama logout si la ruta actual es login (evita loop)', async () => {
    mockRouter.currentRoute.value.name = 'login'
    const error = { isTokenExpired: true, message: 'Token expirado' }

    await expect(runResponseErrorInterceptor(error))
      .rejects.toMatchObject({ isTokenExpired: true })

    expect(mockLogout).not.toHaveBeenCalled()
    expect(mockHandle).not.toHaveBeenCalled()
  })
})

// ─────────────────────────────────────────────────────────────────────────────
describe('Response interceptor — códigos HTTP', () => {
  it('401: muestra toast y llama logout si NO está en login', async () => {
    mockRouter.currentRoute.value.name = 'dashboard'
    const error = makeAxiosError(401)

    await expect(runResponseErrorInterceptor(error)).rejects.toBeInstanceOf(AxiosError)

    expect(mockHandle).toHaveBeenCalledWith(error, expect.objectContaining({ showToast: true }))
    expect(mockLogout).toHaveBeenCalledOnce()
  })

  it('401: NO llama logout si ya está en login (evita loop)', async () => {
    mockRouter.currentRoute.value.name = 'login'
    const error = makeAxiosError(401)

    await expect(runResponseErrorInterceptor(error)).rejects.toBeInstanceOf(AxiosError)

    expect(mockLogout).not.toHaveBeenCalled()
  })

  it('403: muestra toast con mensaje de permisos', async () => {
    const error = makeAxiosError(403)

    await expect(runResponseErrorInterceptor(error)).rejects.toBeInstanceOf(AxiosError)

    expect(mockHandle).toHaveBeenCalledWith(
      error,
      expect.objectContaining({ showToast: true, customMessage: expect.stringContaining('permisos') })
    )
    expect(mockLogout).not.toHaveBeenCalled()
  })

  it('404: llama handleSilent (sin toast)', async () => {
    const error = makeAxiosError(404)

    await expect(runResponseErrorInterceptor(error)).rejects.toBeInstanceOf(AxiosError)

    expect(mockHandleSilent).toHaveBeenCalledWith(error)
    expect(mockHandle).not.toHaveBeenCalled()
  })

  it.each([500, 502, 503])(
    '%i: muestra toast con mensaje de error del servidor',
    async (status) => {
      const error = makeAxiosError(status)

      await expect(runResponseErrorInterceptor(error)).rejects.toBeInstanceOf(AxiosError)

      expect(mockHandle).toHaveBeenCalledWith(
        error,
        expect.objectContaining({ showToast: true, customMessage: expect.stringContaining('servidor') })
      )
    }
  )
})

// ─────────────────────────────────────────────────────────────────────────────
describe('Response interceptor — errores de red y timeout', () => {
  it('Network Error: muestra toast de sin conexión', async () => {
    const error = makeAxiosError(undefined, undefined, 'Network Error')
    error.response = undefined

    await expect(runResponseErrorInterceptor(error)).rejects.toBeInstanceOf(AxiosError)

    expect(mockHandle).toHaveBeenCalledWith(
      error,
      expect.objectContaining({ showToast: true, customMessage: expect.stringContaining('conexión') })
    )
  })

  it('ECONNABORTED: muestra toast de timeout', async () => {
    // Necesita response definido para no entrar al branch "Network Error / !error.response" antes
    const error = makeAxiosError(undefined, 'ECONNABORTED')
    error.response = { status: 0, data: {}, headers: {}, config: {} as any, statusText: '' }

    await expect(runResponseErrorInterceptor(error)).rejects.toBeInstanceOf(AxiosError)

    expect(mockHandle).toHaveBeenCalledWith(
      error,
      expect.objectContaining({ showToast: true, customMessage: expect.stringContaining('tardó') })
    )
  })

  it('error sin status ni código conocido: rechaza sin llamar handle', async () => {
    const error = makeAxiosError(422)

    await expect(runResponseErrorInterceptor(error)).rejects.toBeInstanceOf(AxiosError)

    expect(mockHandle).not.toHaveBeenCalled()
    expect(mockHandleSilent).not.toHaveBeenCalled()
  })
})

// ─────────────────────────────────────────────────────────────────────────────
describe('configureAxiosDefaults', () => {
  it('configura timeout en 30000ms', () => {
    configureAxiosDefaults()
    expect(axios.defaults.timeout).toBe(30000)
  })

  it('configura Accept: application/json', () => {
    configureAxiosDefaults()
    expect(axios.defaults.headers.common['Accept']).toBe('application/json')
  })

  it('configura withCredentials en false', () => {
    configureAxiosDefaults()
    expect(axios.defaults.withCredentials).toBe(false)
  })

  it('usa VITE_API_URL si está definida', () => {
    import.meta.env.VITE_API_URL = 'https://mi-api.com/'
    configureAxiosDefaults()
    expect(axios.defaults.baseURL).toBe('https://mi-api.com/')
    delete import.meta.env.VITE_API_URL
  })

  it('usa fallback localhost si VITE_API_URL no está definida', () => {
    delete import.meta.env.VITE_API_URL
    configureAxiosDefaults()
    expect(axios.defaults.baseURL).toBe('https://localhost:7145/')
  })
})