import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth';
import { normalize } from '@/utils/string';
import HomeView from '../views/HomeView.vue'
import LoginView from '@/views/LoginView.vue';

declare module 'vue-router' {
  interface RouteMeta {
    free?: boolean;
    requiresAuth?: boolean;
    module?: string;
  }
}

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "home",
    component: HomeView,
    meta: {
      requiresAuth: true,
    },
  },
  {
    path: "/marcas",
    name: "brand",
    component: () => import("@/views/BrandView.vue"),
    meta: {
      requiresAuth: true,
      module: "marcas",
    },
  },
  {
    path: "/categorias",
    name: "category",
    component: () => import("@/views/CategoryView.vue"),
    meta: {
      requiresAuth: true,
      module: "categorias",
    },
  },
  {
    path: "/clientes",
    name: "customer",
    component: () => import("@/views/CustomerView.vue"),
    meta: {
      requiresAuth: true,
      module: "clientes",
    },
  },
  {
    path: "/salidas",
    name: "goodsissue",
    component: () => import("@/views/GoodsIssueView.vue"),
    meta: {
      requiresAuth: true,
      module: "salida de productos",
    },
  },
  {
    path: "/entradas",
    name: "goodsreceipt",
    component: () => import("@/views/GoodsReceiptView.vue"),
    meta: {
      requiresAuth: true,
      module: "entrada de productos",
    },
  },
  {
    path: "/inventario",
    name: "inventory",
    component: () => import("@/views/InventoryView.vue"),
    meta: {
      requiresAuth: true,
      module: "inventario",
    },
  },
  {
    path: "/inicio",
    name: "login",
    component: LoginView,
    meta: {
      free: true,
    },
  },
  {
    path: "/modulos",
    name: "module",
    component: () => import("@/views/ModuleView.vue"),
    meta: {
      requiresAuth: true,
      module: "modulos",
    },
  },
  {
    path: "/permisos",
    name: "permission",
    component: () => import("@/views/PermissionView.vue"),
    meta: {
      requiresAuth: true,
      module: "permisos",
    },
  },
  {
    path: "/productos",
    name: "product",
    component: () => import("@/views/ProductView.vue"),
    meta: {
      requiresAuth: true,
      module: "productos",
    },
  },
  {
    path: "/roles",
    name: "role",
    component: () => import("@/views/RoleView.vue"),
    meta: {
      requiresAuth: true,
      module: "roles",
    },
  },
  {
    path: "/establecimientos",
    name: "store",
    component: () => import("@/views/StoreView.vue"),
    meta: {
      requiresAuth: true,
      module: "establecimientos",
    },
  },
  {
    path: "/proveedores",
    name: "supplier",
    component: () => import("@/views/SupplierView.vue"),
    meta: {
      requiresAuth: true,
      module: "proveedores",
    },
  },
  {
    path: "/traspasos",
    name: "transfer",
    component: () => import("@/views/TransferView.vue"),
    meta: {
      requiresAuth: true,
      module: "traspaso de productos",
    },
  },
  {
    path: "/usuarios",
    name: "user",
    component: () => import("@/views/UserView.vue"),
    meta: {
      requiresAuth: true,
      module: "usuarios",
    },
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()

  // Esperar a que la autenticación esté inicializada
  if (!authStore.authInitialized) {
    await authStore.initializeAuth()
  }

  const currentUser = authStore.currentUser

  // Rutas libres
  if (to.matched.some(record => record.meta.free)) {
    if (currentUser && to.name === 'login') {
      next({ name: 'home' })
      return
    }
    next()
    return
  }

  // Verificar autenticación
  if (!currentUser) {
    next({ name: 'login' })
    return
  }

  // Rutas que requieren autenticación pero no permisos específicos
  if (to.matched.some(record => record.meta.requiresAuth && !record.meta.module)) {
    next()
    return
  }

  // Verificar permisos del módulo
  const routeWithModule = to.matched.find(record => record.meta.module)
  
  if (routeWithModule && routeWithModule.meta.module) {
    const module = routeWithModule.meta.module
    const normalizedRouteModule = normalize(module)
    
    const hasModuleAccess = currentUser.permissions.some(
      (p: { module: string; action: string }) => 
        normalize(p.module) === normalizedRouteModule
    )
    
    if (hasModuleAccess) {
      next()
    } else {
      next({ name: 'home' })
    }
  } else {
    next()
  }
});

export default router