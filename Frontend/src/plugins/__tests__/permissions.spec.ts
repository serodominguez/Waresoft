import { describe, it, expect, vi, beforeEach } from 'vitest'
import { createApp, defineComponent } from 'vue'

// ─── vi.hoisted ───────────────────────────────────────────────────────────────
const { mockHasPermission } = vi.hoisted(() => ({
  mockHasPermission: vi.fn(),
}))

vi.mock('@/stores/authStore', () => ({
  useAuthStore: () => ({ hasPermission: mockHasPermission }),
}))

// ─── Import bajo test ─────────────────────────────────────────────────────────
import permissionPlugin from '@/plugins/permissions'

// ─── Helpers ──────────────────────────────────────────────────────────────────
const StubComponent = defineComponent({ template: '<div><slot /></div>' })

/**
 * Monta un elemento con la directiva v-permission y devuelve el HTMLElement
 * resultante para inspeccionar su estilo.
 */
function mountWithDirective(
  bindingValue: { module?: string; action?: string } | undefined,
  hook: 'mounted' | 'updated' = 'mounted'
) {
  const el = document.createElement('div')

  const app = createApp(StubComponent)
  app.use(permissionPlugin)

  // Acceder a la directiva registrada internamente
  const directive = (app._context.directives as any)['permission']

  const binding = { value: bindingValue } as any
  directive[hook](el, binding)

  return el
}

// ─────────────────────────────────────────────────────────────────────────────
describe('permissionPlugin — directiva v-permission', () => {
  beforeEach(() => {
    mockHasPermission.mockReset()
  })

  // ── mounted ─────────────────────────────────────────────────────────────────
  describe('hook mounted', () => {
    it('no oculta el elemento si el usuario tiene el permiso', () => {
      mockHasPermission.mockReturnValue(true)

      const el = mountWithDirective({ module: 'productos', action: 'read' })

      expect(el.style.display).not.toBe('none')
      expect(mockHasPermission).toHaveBeenCalledWith('productos', 'read')
    })

    it('oculta el elemento (display:none) si el usuario NO tiene el permiso', () => {
      mockHasPermission.mockReturnValue(false)

      const el = mountWithDirective({ module: 'productos', action: 'read' })

      expect(el.style.display).toBe('none')
    })

    it('registra un error en consola y no oculta si falta module', () => {
      const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {})

      const el = mountWithDirective({ module: '', action: 'read' })

      expect(consoleSpy).toHaveBeenCalledWith(expect.stringContaining('v-permission'))
      expect(el.style.display).not.toBe('none')
      expect(mockHasPermission).not.toHaveBeenCalled()

      consoleSpy.mockRestore()
    })

    it('registra un error en consola y no oculta si falta action', () => {
      const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {})

      const el = mountWithDirective({ module: 'productos', action: '' })

      expect(consoleSpy).toHaveBeenCalledWith(expect.stringContaining('v-permission'))
      expect(el.style.display).not.toBe('none')
      expect(mockHasPermission).not.toHaveBeenCalled()

      consoleSpy.mockRestore()
    })
  })

  // ── updated ──────────────────────────────────────────────────────────────────
  describe('hook updated', () => {
    it('oculta el elemento si el usuario pierde el permiso', () => {
      mockHasPermission.mockReturnValue(false)

      const el = mountWithDirective({ module: 'productos', action: 'read' }, 'updated')

      expect(el.style.display).toBe('none')
    })

    it('restaura el elemento (display:"") si el usuario recupera el permiso', () => {
      // Primero lo ocultamos manualmente para simular estado previo
      const el = document.createElement('div')
      el.style.display = 'none'

      mockHasPermission.mockReturnValue(true)

      const app = createApp(StubComponent)
      app.use(permissionPlugin)
      const directive = (app._context.directives as any)['permission']
      directive.updated(el, { value: { module: 'productos', action: 'read' } })

      expect(el.style.display).toBe('')
    })

    it('no hace nada si falta module o action en updated', () => {
      mockHasPermission.mockReturnValue(false)

      const el = document.createElement('div')
      el.style.display = 'block'

      const app = createApp(StubComponent)
      app.use(permissionPlugin)
      const directive = (app._context.directives as any)['permission']
      directive.updated(el, { value: { module: '', action: 'read' } })

      // Sin module → return temprano, no cambia el display
      expect(el.style.display).toBe('block')
      expect(mockHasPermission).not.toHaveBeenCalled()
    })

    it('llama hasPermission con los parámetros correctos al actualizar', () => {
      mockHasPermission.mockReturnValue(true)

      mountWithDirective({ module: 'ventas', action: 'write' }, 'updated')

      expect(mockHasPermission).toHaveBeenCalledWith('ventas', 'write')
    })
  })
})

// ─────────────────────────────────────────────────────────────────────────────
describe('permissionPlugin — $hasPermission global property', () => {
  beforeEach(() => {
    mockHasPermission.mockReset()
  })

  function getGlobalHelper() {
    const app = createApp(StubComponent)
    app.use(permissionPlugin)
    return app.config.globalProperties.$hasPermission as (m: string, a: string) => boolean
  }

  it('devuelve true si el usuario tiene el permiso', () => {
    mockHasPermission.mockReturnValue(true)

    const result = getGlobalHelper()('productos', 'read')

    expect(result).toBe(true)
    expect(mockHasPermission).toHaveBeenCalledWith('productos', 'read')
  })

  it('devuelve false si el usuario NO tiene el permiso', () => {
    mockHasPermission.mockReturnValue(false)

    const result = getGlobalHelper()('admin', 'delete')

    expect(result).toBe(false)
  })
})