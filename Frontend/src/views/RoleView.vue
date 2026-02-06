<template>
  <div>
    <RoleList :roles="roles" :loading="loading" :totalRoles="totalRoles" :downloadingExcel="downloadingExcel"
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-role="openForm"
      @fetch-roles="fetchRoles" @search-roles="searchRoles" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf" />

    <RoleForm v-model="form" :role="selectedRole" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedRole?.idRole || 0" :item="selectedRole?.roleName || ''"
      :action="action" moduleName="role" entityName="Role" name="Rol" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useRoleStore } from '@/stores/roleStore';
import { useAuthStore } from '@/stores/auth';
import { Role } from '@/interfaces/roleInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import RoleList from '@/components/Role/RoleList.vue';
import RoleForm from '@/components/Role/RoleForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const roleStore = useRoleStore();
const authStore = useAuthStore();

const toast = useToast();

const filterMap: Record<string, number> = { "Rol": 1 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Rol', filterMap);

const currentPage = ref(1);
const itemsPerPage = ref(10);

const search = ref<string | null>(null);
const drawer = ref(false);

const form = ref(false);
const modal = ref(false);

const selectedRole = ref<Role | null>(null);

const action = ref<0 | 1 | 2>(0);

const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

const roles = computed(() => roleStore.roles);
const loading = computed(() => roleStore.loading);
const totalRoles = computed(() => roleStore.totalRoles);

const canCreate = computed((): boolean => authStore.hasPermission('roles', 'crear'));
const canRead = computed((): boolean => authStore.hasPermission('roles', 'leer'));
const canEdit = computed((): boolean => authStore.hasPermission('roles', 'editar'));
const canDelete = computed((): boolean => authStore.hasPermission('roles', 'eliminar'));
const canDownload = computed((): boolean => authStore.hasPermission('roles', 'descargar'));

const openModal = (payload: { role: Role, action: 0 | 1 | 2 }) => {
  selectedRole.value = payload.role;
  action.value = payload.action;
  modal.value = true;
};

const openForm = (role?: Role) => {
  selectedRole.value = role ? { ...role } : {
    idRole: null,
    roleName: '',
    auditCreateDate: '',
    statusRole: ''
  };
  form.value = true;
};

const fetchRoles = async (params?: any) => {
  try {
    await roleStore.fetchRoles(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchRoles = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await roleStore.fetchRoles({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar roles');
  }
};

const refreshRoles = () => {
  if (search.value?.trim()) {
    searchRoles({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchRoles();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshRoles();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshRoles();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await roleStore.downloadRolesExcel({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    toast.success('Archivo descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo Excel');
  } finally {
    downloadingExcel.value = false;
  }
};

const downloadPdf = async (params: any) => {
  downloadingPdf.value = true;
  try {
    await roleStore.downloadRolesPdf({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    toast.success('Archivo PDF descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo PDF');
  } finally {
    downloadingPdf.value = false;
  }
};

const handleSaved = () => {
  fetchRoles();
};

const handleActionCompleted = () => {
  fetchRoles();
};

onMounted(() => {
  fetchRoles();
});
</script>