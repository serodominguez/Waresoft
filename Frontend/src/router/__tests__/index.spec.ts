import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'

// ─── Mock authStore ───────────────────────────────────────────────────────────
// Debe declararse ANTES de cualquier import que use el router,
// porque vi.mock se eleva (hoisted) al tope del archivo.
const mockAuthStore = {
  authInitialized: true,
  currentUser: null as null | { permissions: { module: string; action: string }[] },
  initializeAuth: vi.fn(),
}

vi.mock('@/stores/authStore', () => ({
  useAuthStore: () => mockAuthStore,
}))

vi.mock('@/utils/string', () => ({
  normalize: (s: string) => s.toLowerCase().trim(),
}))

// ─── Mock de componentes: Dashboard y Login (importados directamente) ─────────
vi.mock('@/components/Dashboard/Dashboard.vue', () => ({ default: { template: '<div />' } }))
vi.mock('@/components/Login/Login.vue',         () => ({ default: { template: '<div />' } }))

// ─── Mock de TODAS las views con lazy-import ─────────────────────────────────
// Sin esto, Vitest intenta cargar los .css de Vuetify y falla con
// ERR_UNKNOWN_FILE_EXTENSION.
vi.mock('@/views/BrandView.vue',       () => ({ default: { template: '<div />' } }))
vi.mock('@/views/CategoryView.vue',    () => ({ default: { template: '<div />' } }))
vi.mock('@/views/ConsolidatedView.vue',() => ({ default: { template: '<div />' } }))
vi.mock('@/views/CustomerView.vue',    () => ({ default: { template: '<div />' } }))
vi.mock('@/views/GoodsIssueView.vue',  () => ({ default: { template: '<div />' } }))
vi.mock('@/views/GoodsReceiptView.vue',() => ({ default: { template: '<div />' } }))
vi.mock('@/views/InventoryView.vue',   () => ({ default: { template: '<div />' } }))
vi.mock('@/views/KardexView.vue',      () => ({ default: { template: '<div />' } }))
vi.mock('@/views/ModuleView.vue',      () => ({ default: { template: '<div />' } }))
vi.mock('@/views/PermissionView.vue',  () => ({ default: { template: '<div />' } }))
vi.mock('@/views/ProductView.vue',     () => ({ default: { template: '<div />' } }))
vi.mock('@/views/RoleView.vue',        () => ({ default: { template: '<div />' } }))
vi.mock('@/views/StoreView.vue',       () => ({ default: { template: '<div />' } }))
vi.mock('@/views/SupplierView.vue',    () => ({ default: { template: '<div />' } }))
vi.mock('@/views/TransferView.vue',    () => ({ default: { template: '<div />' } }))
vi.mock('@/views/UserView.vue',        () => ({ default: { template: '<div />' } }))

// ─── Helpers ──────────────────────────────────────────────────────────────────
function makeUser(modules: string[] = []) {
  return {
    permissions: modules.map(m => ({ module: m, action: 'read' })),
  }
}

// Importa el router DESPUÉS de resetear módulos para que el guard
// capture el estado actual de mockAuthStore en cada test.
async function buildRouter() {
  vi.resetModules()
  const { default: router } = await import('@/router')
  return router
}

// ─────────────────────────────────────────────────────────────────────────────
describe('Router navigation guard', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    mockAuthStore.authInitialized = true
    mockAuthStore.currentUser = null
    mockAuthStore.initializeAuth.mockClear()
  })

  // ── initializeAuth ──────────────────────────────────────────────────────────
  describe('inicialización de auth', () => {
    it('llama initializeAuth() si authInitialized es false', async () => {
      mockAuthStore.authInitialized = false
      mockAuthStore.initializeAuth.mockImplementation(() => {
        mockAuthStore.authInitialized = true
        mockAuthStore.currentUser = null
      })

      const router = await buildRouter()
      await router.push('/inicio')
      await router.isReady()

      expect(mockAuthStore.initializeAuth).toHaveBeenCalledOnce()
    })

    it('no llama initializeAuth() si authInitialized es true', async () => {
      const router = await buildRouter()
      await router.push('/inicio')
      await router.isReady()

      expect(mockAuthStore.initializeAuth).not.toHaveBeenCalled()
    })
  })

  // ── Rutas libres ────────────────────────────────────────────────────────────
  describe('rutas libres (meta.free)', () => {
    it('permite el acceso a /inicio sin autenticación', async () => {
      const router = await buildRouter()
      await router.push('/inicio')

      expect(router.currentRoute.value.name).toBe('login')
    })

    it('redirige a dashboard si el usuario ya está autenticado e intenta ir a /inicio', async () => {
      mockAuthStore.currentUser = makeUser()

      const router = await buildRouter()
      await router.push('/inicio')

      expect(router.currentRoute.value.name).toBe('dashboard')
    })
  })

  // ── Rutas protegidas — sin módulo ───────────────────────────────────────────
  describe('rutas protegidas sin módulo específico', () => {
    it('redirige a /inicio si el usuario no está autenticado', async () => {
      const router = await buildRouter()
      await router.push('/')

      expect(router.currentRoute.value.name).toBe('login')
    })

    it('permite el acceso al dashboard si el usuario está autenticado', async () => {
      mockAuthStore.currentUser = makeUser()

      const router = await buildRouter()
      await router.push('/')

      expect(router.currentRoute.value.name).toBe('dashboard')
    })
  })

  // ── Rutas protegidas con módulo ─────────────────────────────────────────────
  describe('rutas protegidas con módulo (meta.module)', () => {
    const protectedRoutes = [
      { path: '/marcas',      name: 'brand',        module: 'marcas'                },
      { path: '/categorias',  name: 'category',     module: 'categorias'            },
      { path: '/clientes',    name: 'customer',     module: 'clientes'              },
      { path: '/productos',   name: 'product',      module: 'productos'             },
      { path: '/proveedores', name: 'supplier',     module: 'proveedores'           },
      { path: '/roles',       name: 'role',         module: 'roles'                 },
      { path: '/usuarios',    name: 'user',         module: 'usuarios'              },
      { path: '/unidades',    name: 'store',        module: 'unidades'              },
      { path: '/modulos',     name: 'module',       module: 'modulos'               },
      { path: '/permisos',    name: 'permission',   module: 'permisos'              },
      { path: '/inventario',  name: 'inventory',    module: 'inventario'            },
      { path: '/consolidado', name: 'consolidated', module: 'inventario'            },
      { path: '/kardex',      name: 'kardex',       module: 'inventario'            },
      { path: '/salidas',     name: 'goodsissue',   module: 'salida de productos'   },
      { path: '/entradas',    name: 'goodsreceipt', module: 'entrada de productos'  },
      { path: '/traspasos',   name: 'transfer',     module: 'traspaso de productos' },
    ]

    it.each(protectedRoutes)(
      'redirige a login si no está autenticado — $path',
      async ({ path }) => {
        const router = await buildRouter()
        await router.push(path)

        expect(router.currentRoute.value.name).toBe('login')
      }
    )

    it.each(protectedRoutes)(
      'permite acceso con el permiso correcto — $path',
      async ({ path, name, module }) => {
        mockAuthStore.currentUser = makeUser([module])

        const router = await buildRouter()
        await router.push(path)

        expect(router.currentRoute.value.name).toBe(name)
      }
    )

    it.each(protectedRoutes)(
      'redirige a dashboard si el usuario no tiene el permiso del módulo — $path',
      async ({ path }) => {
        mockAuthStore.currentUser = makeUser(['otro modulo'])

        const router = await buildRouter()
        await router.push(path)

        expect(router.currentRoute.value.name).toBe('dashboard')
      }
    )
  })

  // ── Rutas de detalle con parámetro ─────────────────────────────────────────
  describe('rutas de detalle (:id)', () => {
    const detailRoutes = [
      { path: '/salidas/123',   name: 'goodsissue-detail',  module: 'salida de productos'   },
      { path: '/entradas/456',  name: 'goodsreceipt-detail',module: 'entrada de productos'  },
      { path: '/traspasos/789', name: 'transfer-detail',    module: 'traspaso de productos' },
    ]

    it.each(detailRoutes)(
      'permite acceso con permiso y resuelve el parámetro id — $path',
      async ({ path, name, module }) => {
        mockAuthStore.currentUser = makeUser([module])

        const router = await buildRouter()
        await router.push(path)

        expect(router.currentRoute.value.name).toBe(name)
        expect(router.currentRoute.value.params.id).toBeDefined()
      }
    )

    it.each(detailRoutes)(
      'redirige a dashboard sin permiso — $path',
      async ({ path }) => {
        mockAuthStore.currentUser = makeUser(['otro modulo'])

        const router = await buildRouter()
        await router.push(path)

        expect(router.currentRoute.value.name).toBe('dashboard')
      }
    )
  })

  // ── Normalización de módulos ────────────────────────────────────────────────
  describe('normalización de nombres de módulo', () => {
    it('coincide aunque el permiso tenga mayúsculas o espacios extra', async () => {
      mockAuthStore.currentUser = {
        permissions: [{ module: '  Marcas  ', action: 'read' }],
      }

      const router = await buildRouter()
      await router.push('/marcas')

      expect(router.currentRoute.value.name).toBe('brand')
    })
  })
})