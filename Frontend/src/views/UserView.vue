<template>
  <div>
    <UserList :users="users" :loading="loading" :totalUsers="totalUsers" :downloadingExcel="downloadingExcel"
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-user="openForm"
      @fetch-users="fetchUsers" @search-users="searchUsers" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf"
      @clear-filters="clearFilters" />

    <UserForm v-model="form" :user="selectedUser" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedUser?.idUser || 0" :item="selectedUser?.userName || ''"
      :action="action" moduleName="user" entityName="User" name="Usuario" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useUserStore } from '@/stores/userStore';
import { useAuthStore } from '@/stores/authStore';
import { User } from '@/interfaces/userInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import { usePagination } from '@/composables/usePagination';
import UserList from '@/components/User/UserList.vue';
import UserForm from '@/components/User/UserForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const userStore = useUserStore();
const authStore = useAuthStore();
const toast     = useToast();

const filterMap: Record<string, number> = { "Usuario": 1, "Nombres": 2, "Apellidos": 3, "Tienda": 4, "Rol": 5 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Usuario', filterMap);

const search        = ref<string | null>(null);
const drawer        = ref(false);
const form          = ref(false);
const modal         = ref(false);
const selectedUser  = ref<User | null>(null);
const action        = ref<0 | 1 | 2>(0);
const downloadingExcel = ref(false);
const downloadingPdf   = ref(false);

const { currentPage, itemsPerPage, updateItemsPerPage, changePage } = usePagination(
  (params) => {
    userStore.fetchAll({
      pageNumber:  params.pageNumber,
      pageSize:    params.pageSize,
      ...getFilterParams(search.value),
    });
  }
);

const users       = computed(() => userStore.list);
const loading     = computed(() => userStore.loading);
const totalUsers  = computed(() => userStore.total);

const canCreate   = computed(() => authStore.hasPermission('usuarios', 'crear'));
const canRead     = computed(() => authStore.hasPermission('usuarios', 'leer'));
const canEdit     = computed(() => authStore.hasPermission('usuarios', 'editar'));
const canDelete   = computed(() => authStore.hasPermission('usuarios', 'eliminar'));
const canDownload = computed(() => authStore.hasPermission('usuarios', 'descargar'));

const openModal = (payload: { user: User; action: 0 | 1 | 2 }) => {
  selectedUser.value = payload.user;
  action.value       = payload.action;
  modal.value        = true;
};

const openForm = (user?: User) => {
  selectedUser.value = user ? { ...user } : {
    idUser:           null,
    userName:         '',
    password:         '',
    passwordHash:     '',
    names:            '',
    lastNames:        '',
    identificationNumber: '',
    phoneNumber:      null,
    idRole:           null,
    roleName:         '',
    idStore:          null,
    storeName:        '',
    auditCreateDate:  '',
    statusUser:       '',
    updatePassword:   false
  };
  form.value = true;
};

const fetchUsers = async () => {
  try {
    await userStore.fetchAll({
      pageNumber:  currentPage.value,
      pageSize:    itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0,
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchUsers = async (params: {
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
    await userStore.fetchAll({
      pageNumber: 1,
      pageSize:   itemsPerPage.value,
      ...getFilterParams(params.search),
    });
  } catch (error) {
    handleApiError(error, 'Error al buscar usuarios');
  }
};

const clearFilters = () => {
  selectedFilter.value = 'Usuario';
  state.value          = 'Activos';
  startDate.value      = null;
  endDate.value        = null;
  search.value         = null;
  currentPage.value    = 1;
  fetchUsers();
};

const downloadExcel = async (params: { search: string | null }) => {
  downloadingExcel.value = true;
  try {
    await userStore.downloadExcel({
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
    await userStore.downloadPdf({
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
  fetchUsers();
};

const handleActionCompleted = () => {
  fetchUsers();
};

onMounted(() => {
  fetchUsers();
});
</script>