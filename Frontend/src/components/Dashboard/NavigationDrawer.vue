<template>
  <v-navigation-drawer v-model="isOpen" app temporary>
    <v-list>
      <v-list-item variant="plain" :to="{ name: 'home' }">
        <v-list-item prepend-icon="home" title="Home"></v-list-item>
      </v-list-item>

      <v-list-group v-if="hasStorePermissions">
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Almacén"></v-list-item>
        </template>
        
        <v-list-item 
          v-for="link in visibleDirectStoreLinks" 
          :key="link.text" 
          :to="link.route"
          rounded="xl" 
          class="ma-0 ml-n10"
        >
          <template v-slot:prepend>
            <v-icon :icon="link.icon" style="font-size: 20px; margin-right: -20px;"></v-icon>
          </template>
          <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
        </v-list-item>

        <v-list-group v-if="hasMovementsPermissions" sub-group>
          <template v-slot:activator="{ props }">
            <v-list-item v-bind="props" title="Movimientos" rounded="xl"  class="ml-n10"></v-list-item>
          </template>
          <v-list-item 
            v-for="link in visibleMovementLinks" 
            :key="link.text" 
            :to="link.route" 
            rounded="xl"
            class="ma-0 ml-n16"
          >
            <template v-slot:prepend>
              <v-icon :icon="link.icon" style="font-size: 20px; margin-right: -20px;"></v-icon>
            </template>
            <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
          </v-list-item>
        </v-list-group>
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
        <v-list-item 
          v-for="link in visibleAccessLinks" 
          :key="link.text" 
          :to="link.route"
          rounded="xl" 
          class="ma-0 ml-n10"
        >
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
        <v-list-item 
          v-for="link in visibleAuthorizationLinks" 
          :key="link.text" 
          :to="link.route"
          rounded="xl" 
          class="ma-0 ml-n10"
        >
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
const linkDirectStore: Link[] = [
  { icon: 'category', text: 'Categorías', route: '/categorias', module: 'categorias' },
  { icon: 'contact_page', text: 'Clientes', route: '/clientes', module: 'clientes' },
  { icon: 'store', text: 'Establecimientos', route: '/establecimientos', module: 'establecimientos' },
  { icon: 'warehouse', text: 'Inventario', route: '/inventario', module: 'inventario' },
  { icon: 'copyright', text: 'Marcas', route: '/marcas', module: 'marcas' },
  { icon: 'inventory_2', text: 'Productos', route: '/productos', module: 'productos' },
  { icon: 'contacts', text: 'Proveedores', route: '/proveedores', module: 'proveedores' },
];

// Enlaces del subgrupo Movimientos
const linkMovements: Link[] = [
  { icon: 'add_shopping_cart', text: 'Entradas', route: '/entradas', module: 'entrada de productos' },
  { icon: 'remove_shopping_cart', text: 'Salidas', route: '/salidas', module: 'salida de productos' },
  { icon: 'shopping_cart_checkout', text: 'Traspasos', route: '/traspasos', module: 'traspaso de productos' },
];

// Enlaces del menú de Consultas
const linkQueries: Link[] = [
  { icon: 'assignment', text: 'Consolidado', route: '/consolidado', module: 'inventario' },
];

// Enlaces del menú de Accesos
const linkAccess: Link[] = [
  { icon: 'supervisor_account', text: 'Roles', route: '/roles', module: 'roles' },
  { icon: 'person', text: 'Usuarios', route: '/usuarios', module: 'usuarios' },
];

// Enlaces del menú de Autorización
const linkAuthorization: Link[] = [
  { icon: 'app_registration', text: 'Módulos', route: '/modulos', module: 'modulos' },
  { icon: 'manage_accounts', text: 'Permisos', route: '/permisos', module: 'permisos' },
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

// Filtra los enlaces directos de Almacén según los permisos del usuario
const visibleDirectStoreLinks = computed((): Link[] => 
  linkDirectStore.filter(link => hasModuleAccess(link.module))
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
  visibleDirectStoreLinks.value.length > 0 || visibleMovementLinks.value.length > 0
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