<template>
  <div>
    <RoleList :roles="roles" :loading="loading" :totalRoles="totalRoles" :downloadingExcel="downloadingExcel"
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-role="openForm"
      @fetch-roles="fetchRoles" @search-roles="searchRoles" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf"
      @clear-filters="clearFilters" />

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
import { useAuthStore } from '@/stores/authStore';
import { Role } from '@/interfaces/roleInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import { usePagination } from '@/composables/usePagination';
import RoleList from '@/components/Role/RoleList.vue';
import RoleForm from '@/components/Role/RoleForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const roleStore = useRoleStore();
const authStore = useAuthStore();
const toast     = useToast();

const filterMap: Record<string, number> = { "Rol": 1 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Rol', filterMap);

const search        = ref<string | null>(null);
const drawer        = ref(false);
const form          = ref(false);
const modal         = ref(false);
const selectedRole  = ref<Role | null>(null);
const action        = ref<0 | 1 | 2>(0);
const downloadingExcel = ref(false);
const downloadingPdf   = ref(false);

const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    roleStore.fetchAll({
      pageNumber:  params.pageNumber,
      pageSize:    params.pageSize,
      ...getFilterParams(search.value),
    });
  }
);

const roles       = computed(() => roleStore.list);
const loading     = computed(() => roleStore.loading);
const totalRoles  = computed(() => roleStore.total);

const canCreate   = computed(() => authStore.hasPermission('roles', 'crear'));
const canRead     = computed(() => authStore.hasPermission('roles', 'leer'));
const canEdit     = computed(() => authStore.hasPermission('roles', 'editar'));
const canDelete   = computed(() => authStore.hasPermission('roles', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('roles', 'descargar'));

const openModal = (payload: { role: Role; action: 0 | 1 | 2 }) => {
  selectedRole.value = payload.role;
  action.value       = payload.action;
  modal.value        = true;
};

const openForm = (role?: Role) => {
  selectedRole.value = role ? { ...role } : {
    idRole:        null,
    roleName:      '',
    auditCreateDate: '',
    statusRole:    ''
  };
  form.value = true;
};

const fetchRoles = async () => {
  try {
    await roleStore.fetchAll({
      pageNumber:  currentPage.value,
      pageSize:    itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0,
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchRoles = async (params: {
  search: string | null;
  selectedFilter: string;
  state: string;
  startDate: Date | null;
  endDate: Date | null;
}) => {
  search.value          = params.search;
  selectedFilter.value  = params.selectedFilter;
  state.value           = params.state;
  startDate.value       = params.startDate;
  endDate.value         = params.endDate;
  currentPage.value     = 1;

  try {
    await roleStore.fetchAll({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
    });
  } catch (error) {
    handleApiError(error, 'Error al buscar roles');
  }
};

const clearFilters = () => {
  selectedFilter.value = 'Rol';
  state.value          = 'Activos';
  startDate.value      = null;
  endDate.value        = null;
  search.value         = null;
  currentPage.value    = 1;
  fetchRoles();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await roleStore.downloadExcel({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
    });
    toast.success('Archivo descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo Excel');
  } finally {
    downloadingExcel.value = false;
  }
};

const downloadPdf = async (params: { search: string | null }) => {
  downloadingPdf.value = true;
  try {
    await roleStore.downloadPdf({
      pageNumber: currentPage.value,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
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