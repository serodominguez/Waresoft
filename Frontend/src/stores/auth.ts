import { defineStore } from 'pinia'
import { jwtDecode } from 'jwt-decode'
import axios from 'axios'
import router from '@/router/index'
import { normalize } from '@/utils/string'

interface UserPermission {
  module: string
  action: string
}

interface JwtPayload {
  userId: string
  userName: string
  role: string
  storeName: string
  storeType: string
  storeId: string
  nbf: number
  exp: number
  iss: string
  aud: string
}

interface CurrentUser {
  userId: number
  userName: string
  role: string
  storeId: number
  storeName: string
  storeType: string
  permissions: UserPermission[]
}

interface AuthState {
  token: string | null
  currentUser: CurrentUser | null
  authInitialized: boolean
}

const createUserFromToken = (
  decoded: JwtPayload,
  userId: number,
  permissions: UserPermission[] = []
): CurrentUser => ({
  userId,
  userName: decoded.userName,
  role: decoded.role,
  storeId: parseInt(decoded.storeId, 10),
  storeType: decoded.storeType,
  storeName: decoded.storeName,
  permissions,
})

const isTokenExpired = (token: string): boolean => {
  try {
    const decoded = jwtDecode<JwtPayload>(token)
    return decoded.exp < Date.now() / 1000
  } catch {
    return true
  }
}

// Helper para obtener usuario cacheado de localStorage
const getCachedUser = (): CurrentUser | null => {
  try {
    const cached = localStorage.getItem('user')
    if (cached) {
      return JSON.parse(cached) as CurrentUser
    }
  } catch (error) {
    console.error('Error parseando usuario cacheado:', error)
  }
  return null
}

// Helper para limpiar completamente la sesión
const clearSession = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('user')
  delete axios.defaults.headers.common['Authorization']
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token: null,
    currentUser: null,
    authInitialized: false,
  }),

  getters: {
    isAuthenticated: (state): boolean => {
      return !!state.token
    },

    getCurrentUser: (state): CurrentUser | null => {
      return state.currentUser
    },

    hasPermission: (state) => {
      return (module: string, action: string): boolean => {
        if (!state.currentUser?.permissions) return false

        const normalizedModule = normalize(module)
        const normalizedAction = normalize(action)

        return state.currentUser.permissions.some(
          (p) =>
            normalize(p.module) === normalizedModule &&
            normalize(p.action) === normalizedAction
        )
      }
    },
  },

  actions: {
    // Inicializa la autenticación al cargar la app
    async initializeAuth() {
      try {
        const token = localStorage.getItem('token')

        // Sin token → continuar sin usuario
        if (!token) {
          this.authInitialized = true
          return
        }

        // Token expirado → limpiar todo
        if (isTokenExpired(token)) {
          this.token = null
          this.currentUser = null
          clearSession()
          this.authInitialized = true
          return
        }

        // Token válido → restaurar sesión desde localStorage
        this.token = token
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`

        const cachedUser = getCachedUser()

        if (cachedUser) {
          this.currentUser = cachedUser
        } else {
          const decoded = jwtDecode<JwtPayload>(token)
          const userId = parseInt(decoded.userId, 10)
          const userWithoutPermissions = createUserFromToken(decoded, userId, [])
          this.currentUser = userWithoutPermissions
          console.warn('Usuario restaurado sin permisos desde token')
        }

        this.authInitialized = true
      } catch (error) {
        console.error('Error inicializando auth:', error)
        this.token = null
        this.currentUser = null
        clearSession()
        this.authInitialized = true
      }
    },

    // Guarda el token y carga permisos del servidor (solo en LOGIN)
    async saveToken(token: string) {
      try {
        this.token = token
        localStorage.setItem('token', token)
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`

        const decoded = jwtDecode<JwtPayload>(token)
        const userId = parseInt(decoded.userId, 10)

        // Cargar permisos frescos desde el servidor
        await this.loadUserPermissions(decoded, userId)
      } catch (error) {
        console.error('Error al guardar token:', error)
        this.token = null
        clearSession()
        throw error
      }
    },

    // Solo se llama en LOGIN o cuando se necesita refrescar manualmente
    async loadUserPermissions(decoded: JwtPayload, userId: number) {
      try {
        const response = await axios.get('/api/Permission/User')

        const permissions = response.data.isSuccess
          ? response.data.data || []
          : []

        const user = createUserFromToken(decoded, userId, permissions)
        this.currentUser = user
        localStorage.setItem('user', JSON.stringify(user))
      } catch (error) {
        console.error('Error cargando permisos:', error)

        const user = createUserFromToken(decoded, userId, [])
        this.currentUser = user
        localStorage.setItem('user', JSON.stringify(user))
      }
    },

    // Cierra sesión y limpia todo
    logout() {
      this.token = null
      this.currentUser = null
      clearSession()
      router.push({ name: 'login' })
    },
  },
})