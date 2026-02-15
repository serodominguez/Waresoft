<template>
  <div>
    <v-card elevation="2">
      <v-toolbar>
        <v-toolbar-title>Gestión de Permisos</v-toolbar-title>
        <v-spacer></v-spacer>
      </v-toolbar>
      <v-card-text>
        <v-row>
          <v-col cols="12" md="2">
            <v-autocomplete color="indigo" variant="underlined" :items="roles" v-model="selectedRoleId"
              item-title="roleName" item-value="idRole" no-data-text="No hay datos disponibles" label="Rol"
              :loading="loadingRoles" />
          </v-col>
          <v-col cols="4" md="9" class="d-flex align-center gap-2">
            <v-btn color="indigo" @click="loadPermissions" :disabled="!selectedRoleId || loading" :loading="loading">
              Cargar
            </v-btn>
            <v-btn color="green" @click="savePermissions" :disabled="!hasChanges || saving" :loading="saving"
              class="ml-2">
              Guardar
            </v-btn>
            <v-btn color="red" @click="clearPermissions" :disabled="!selectedRoleId || loading" :loading="loading"
              class="ml-2">
              Limpiar
            </v-btn>
          </v-col>
        </v-row>
        <PermissionList :permissions="localPermissions" :loading="loading" @permission-changed="markAsChanged" />
      </v-card-text>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useRoleStore } from '@/stores/roleStore';
import { usePermissionStore } from '@/stores/permissionStore';
import { PermissionsByModule } from '@/interfaces/permissionInterface';
import { handleApiError } from '@/helpers/errorHandler';
import PermissionList from '@/components/Permission/PermissionList.vue';

// Inicialización
const roleStore = useRoleStore();
const permissionStore = usePermissionStore();
const toast = useToast();

// Estado reactivo
const selectedRoleId = ref<number | null>(null);
const localPermissions = ref<PermissionsByModule[]>([]);
const originalPermissions = ref<PermissionsByModule[]>([]);
const hasChanges = ref(false);
const saving = ref(false);

// Computed properties
const roles = computed(() => {
  return Array.isArray(roleStore.roles) ? roleStore.roles : [];
});

const loadingRoles = computed<boolean>(() => roleStore.loading);

const permissionsByModule = computed<PermissionsByModule[]>(
  () => permissionStore.permissionsByModule
);

const permissions = computed(() => permissionStore.permissions);

const loading = computed<boolean>(() => permissionStore.loading);

// Watchers
watch(
  permissionsByModule,
  (newPermissions) => {
    localPermissions.value = JSON.parse(JSON.stringify(newPermissions));
    originalPermissions.value = JSON.parse(JSON.stringify(newPermissions));
    hasChanges.value = false;
  },
  { deep: true }
);

// Métodos
const loadPermissions = async () => {
  if (!selectedRoleId.value) {
    toast.warning('Por favor selecciona un rol');
    return;
  }

  hasChanges.value = false;

  try {
    await permissionStore.fetchPermissionsByRole(selectedRoleId.value);
  } catch (error) {
    handleApiError(error, 'Error al cargar los permisos del rol');
  }
};

const markAsChanged = () => {
  hasChanges.value = true;
};

const savePermissions = async () => {
  if (!hasChanges.value) {
    toast.info('No hay cambios para guardar');
    return;
  }

  saving.value = true;

  try {
    // Construir array de permisos actualizados
    const updatedPermissions = permissions.value.map((perm: any) => {
      const localModule = localPermissions.value.find(
        (lp) => lp.module === perm.moduleName
      );

      if (localModule) {
        const actionKey = perm.actionName.toLowerCase() as 'crear' | 'leer' | 'editar' | 'eliminar' | 'descargar';
        return {
          idPermission: perm.idPermission,
          status: localModule.permissions[actionKey]
        };
      }

      return {
        idPermission: perm.idPermission,
        status: perm.status
      };
    });

    // Llamar al store
    const response = await permissionStore.updatePermissions(updatedPermissions);

    if (response && response.success) {
      toast.success('Permisos actualizados correctamente');
      hasChanges.value = false;

      // Recargar permisos para confirmar
      if (selectedRoleId.value) {
        await permissionStore.fetchPermissionsByRole(selectedRoleId.value);
      }
    } else {
      const errorMsg = response?.message || 'Error al actualizar permisos';
      toast.error(errorMsg);
    }
  } catch (error) {
    handleApiError(error, 'Error al guardar los permisos');
  } finally {
    saving.value = false;
  }
};

const clearPermissions = () => {
  selectedRoleId.value = null;
  localPermissions.value = [];
  originalPermissions.value = [];
  hasChanges.value = false;
  permissionStore.clearPermissions();
};

// Lifecycle hooks
onMounted(() => {
  roleStore.selectRole();
});
</script>