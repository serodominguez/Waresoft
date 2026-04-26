<template>
  <v-navigation-drawer v-model="isOpen" app temporary>
    <v-list>
      <v-list-item variant="plain" :to="{ name: 'home' }">
        <v-list-item prepend-icon="mdi-view-dashboard" title="Dashboard"></v-list-item>
      </v-list-item>

      <v-list-group v-if="hasStorePermissions">
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Almacén"></v-list-item>
        </template>

        <v-list-item v-for="link in visibleStoreLinks" :key="link.text" :to="link.route" rounded="xl"
          class="ma-0 ml-n10">
          <template v-slot:prepend>
            <v-icon :icon="link.icon" style="font-size: 20px; margin-right: -20px;"></v-icon>
          </template>
          <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
        </v-list-item>

        <v-list-group v-if="hasMovementsPermissions" sub-group>
          <template v-slot:activator="{ props }">
            <v-list-item v-bind="props" title="Movimientos" rounded="xl" class="ml-n10"></v-list-item>
          </template>
          <v-list-item v-for="link in visibleMovementLinks" :key="link.text" :to="link.route" rounded="xl"
            class="ma-0 ml-n16">
            <template v-slot:prepend>
              <v-icon :icon="link.icon" style="font-size: 20px; margin-right: -20px;"></v-icon>
            </template>
            <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
          </v-list-item>
        </v-list-group>
      </v-list-group>

      
      <v-list-group v-if="hasSalesPermissions">
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Comercial"></v-list-item>
        </template>

        <v-list-item v-for="link in visibleSalesLinks" :key="link.text" :to="link.route" rounded="xl"
          class="ma-0 ml-n10">
          <template v-slot:prepend>
            <v-icon :icon="link.icon" style="font-size: 20px; margin-right: -20px;"></v-icon>
          </template>
          <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
        </v-list-item>
      </v-list-group>

      <v-list-group v-if="hasQueriesPermissions">
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Consultas"></v-list-item>
        </template>
        <v-list-item v-for="link in visibleQueriesLinks" :key="link.text" :to="link.route" rounded="xl"
          class="ma-0 ml-n10">
          <template v-slot:prepend>
            <v-icon :icon="link.icon" style="font-size: 20px; margin-right: -20px;"></v-icon>
          </template>
          <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
        </v-list-item>
      </v-list-group>

      <v-list-group v-if="hasAccessPermissions">
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Accesos"></v-list-item>
        </template>
        <v-list-item v-for="link in visibleAccessLinks" :key="link.text" :to="link.route" rounded="xl"
          class="ma-0 ml-n10">
          <template v-slot:prepend>
            <v-icon :icon="link.icon" style="font-size: 20px; margin-right: -20px;"></v-icon>
          </template>
          <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
        </v-list-item>
      </v-list-group>

      <v-list-group v-if="hasAuthorizationPermissions">
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Autorización"></v-list-item>
        </template>
        <v-list-item v-for="link in visibleAuthorizationLinks" :key="link.text" :to="link.route" rounded="xl"
          class="ma-0 ml-n10">
          <template v-slot:prepend>
            <v-icon :icon="link.icon" style="font-size: 20px; margin-right: -20px;"></v-icon>
          </template>
          <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
        </v-list-item>
      </v-list-group>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { normalize } from '@/utils/string';

// Interface que define la estructura de un enlace del menú de navegación
interface Link {
  icon: string;      // Icono a mostrar
  text: string;      // Texto del enlace
  route: string;     // Ruta de navegación
  module: string;    // Módulo asociado para validar permisos
}

/**
 * Props del componente
 */
interface Props {
  modelValue?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: false
});

/**
 * Emits del componente
 */
const emit = defineEmits<{
  'update:modelValue': [value: boolean];
}>();

// Inicialización del store
const authStore = useAuthStore();

// Enlaces directos del menú de Almacén
const linkStore: Link[] = [
  { icon: 'mdi-shape', text: 'Categorías', route: '/categorias', module: 'categorias' },
  { icon: 'mdi-warehouse', text: 'Inventario', route: '/inventario', module: 'inventario' },
  { icon: 'mdi-copyright', text: 'Marcas', route: '/marcas', module: 'marcas' },
  { icon: 'mdi-archive', text: 'Productos', route: '/productos', module: 'productos' },
  { icon: 'mdi-contacts', text: 'Proveedores', route: '/proveedores', module: 'proveedores' },
  { icon: 'mdi-store', text: 'Unidades', route: '/unidades', module: 'unidades' },
];

// Enlaces directos del menú de Comercial
const linkSales: Link[] = [
  { icon: 'mdi-account-box', text: 'Clientes', route: '/clientes', module: 'clientes' },
];

// Enlaces del subgrupo Movimientos
const linkMovements: Link[] = [
  { icon: 'mdi-cart-plus', text: 'Entradas', route: '/entradas', module: 'entrada de productos' },
  { icon: 'mdi-cart-minus', text: 'Salidas', route: '/salidas', module: 'salida de productos' },
  { icon: 'mdi-cart-arrow-up', text: 'Traspasos', route: '/traspasos', module: 'traspaso de productos' },
];

// Enlaces del menú de Consultas
const linkQueries: Link[] = [
  { icon: 'mdi-clipboard-text-search-outline', text: 'Consolidado', route: '/consolidado', module: 'inventario' },
  { icon: 'mdi-clipboard-text', text: 'Kardex', route: '/kardex', module: 'inventario' },
];

// Enlaces del menú de Accesos
const linkAccess: Link[] = [
  { icon: 'mdi-account-supervisor', text: 'Roles', route: '/roles', module: 'roles' },
  { icon: 'mdi-account', text: 'Usuarios', route: '/usuarios', module: 'usuarios' },
];

// Enlaces del menú de Autorización
const linkAuthorization: Link[] = [
  { icon: 'mdi-apps', text: 'Módulos', route: '/modulos', module: 'modulos' },
  { icon: 'mdi-account-cog', text: 'Permisos', route: '/permisos', module: 'permisos' },
];

// Verifica si el usuario tiene acceso a un módulo específico
const hasModuleAccess = (module: string): boolean => {
  // Módulos marcados como 'exclude' son siempre visibles (sin restricción)
  if (module === 'exclude') {
    return true;
  }

  // Obtiene el usuario actual del store
  const currentUser = authStore.currentUser;

  // Si no hay usuario o no tiene permisos, deniega el acceso
  if (!currentUser || !currentUser.permissions) {
    return false;
  }

  // Normaliza el nombre del módulo para comparación consistente
  const normalizedModule = normalize(module);

  // Verifica si existe algún permiso del usuario que coincida con el módulo
  return currentUser.permissions.some(
    (p: { module: string; action: string }) => 
      normalize(p.module) === normalizedModule
  );
};

// Computed properties

// Permite la sincronización bidireccional del estado del drawer
const isOpen = computed({
  get: (): boolean => props.modelValue,
  set: (value: boolean): void => emit('update:modelValue', value)
});

// Filtra los enlaces de Almacén según los permisos del usuario
const visibleStoreLinks = computed((): Link[] => 
  linkStore.filter(link => hasModuleAccess(link.module))
);

// Filtra los enlaces de Comercial según los permisos del usuario
const visibleSalesLinks = computed((): Link[] => 
  linkSales.filter(link => hasModuleAccess(link.module))
);

// Filtra los enlaces de Movimientos según los permisos del usuario
const visibleMovementLinks = computed((): Link[] => 
  linkMovements.filter(link => hasModuleAccess(link.module))
);

// Filtra los enlaces de Consultas según los permisos del usuario
const visibleQueriesLinks = computed((): Link[] => 
  linkQueries.filter(link => hasModuleAccess(link.module))
);

// Filtra los enlaces de Accesos según los permisos del usuario
const visibleAccessLinks = computed((): Link[] => 
  linkAccess.filter(link => hasModuleAccess(link.module))
);

// Filtra los enlaces de Autorización según los permisos del usuario
const visibleAuthorizationLinks = computed((): Link[] => 
  linkAuthorization.filter(link => hasModuleAccess(link.module))
);

// Verifica si se debe mostrar la sección de Almacén
const hasStorePermissions = computed((): boolean => 
  visibleStoreLinks.value.length > 0 || visibleMovementLinks.value.length > 0
);

// Verifica si se debe mostrar la sección de Comercial
const hasSalesPermissions = computed((): boolean => 
  visibleSalesLinks.value.length > 0
);

// Verifica si se debe mostrar el subgrupo de Movimientos
const hasMovementsPermissions = computed((): boolean => 
  visibleMovementLinks.value.length > 0
);

// Verifica si se debe mostrar sección de Consultas
const hasQueriesPermissions = computed((): boolean => 
  visibleQueriesLinks.value.length > 0
);

// Verifica si se debe mostrar la sección de Accesos
const hasAccessPermissions = computed((): boolean => 
  visibleAccessLinks.value.length > 0
);

// Verifica si se debe mostrar la sección de Configuración
const hasAuthorizationPermissions = computed((): boolean => 
  visibleAuthorizationLinks.value.length > 0
);
</script>