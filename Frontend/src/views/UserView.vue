<template>
  <div>
    <UserList :users="users" :loading="loading" :totalUsers="totalUsers" :downloadingExcel="downloadingExcel"
      :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :canDownload="canDownload" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-user="openForm"
      @fetch-users="fetchUsers" @search-users="searchUsers" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf" />

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
import { useAuthStore } from '@/stores/auth';
import { User } from '@/interfaces/userInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import UserList from '@/components/User/UserList.vue';
import UserForm from '@/components/User/UserForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const userStore = useUserStore();
const authStore = useAuthStore();

const toast = useToast();

const filterMap: Record<string, number> = {
  "Usuario": 1,
  "Nombres": 2,
  "Apellidos": 3,
  "Tienda": 4,
  "Rol": 5
};
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Usuario', filterMap);

const currentPage = ref(1);
const itemsPerPage = ref(10);
const search = ref<string | null>(null);
const drawer = ref(false);
const form = ref(false);
const modal = ref(false);
const selectedUser = ref<User | null>(null);
const action = ref<0 | 1 | 2>(0);

const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

const users = computed(() => userStore.users);
const loading = computed(() => userStore.loading);
const totalUsers = computed(() => userStore.totalUsers);

const canCreate = computed((): boolean => authStore.hasPermission('usuarios', 'crear'));
const canRead = computed((): boolean => authStore.hasPermission('usuarios', 'leer'));
const canEdit = computed((): boolean => authStore.hasPermission('usuarios', 'editar'));
const canDelete = computed((): boolean => authStore.hasPermission('usuarios', 'eliminar'));
const canDownload = computed((): boolean => authStore.hasPermission('usuarios', 'descargar'));

const openModal = (payload: { user: User, action: 0 | 1 | 2 }) => {
  selectedUser.value = payload.user;
  action.value = payload.action;
  modal.value = true;
};

const openForm = (user?: User) => {
  selectedUser.value = user ? { ...user } : {
    idUser: null,
    userName: '',
    password: '',
    passwordHash: '',
    names: '',
    lastNames: '',
    identificationNumber: '',
    phoneNumber: null,
    idRole: null,
    roleName: '',
    idStore: null,
    storeName: '',
    auditCreateDate: '',
    statusUser: '',
    updatePassword: false
  };
  form.value = true;
};

const fetchUsers = async (params?: any) => {
  try {
    await userStore.fetchUsers(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchUsers = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await userStore.fetchUsers({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar usuarios');
  }
};

const refreshUsers = () => {
  if (search.value?.trim()) {
    searchUsers({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchUsers();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshUsers();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshUsers();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await userStore.downloadUsersExcel({
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
    await userStore.downloadUsersPdf({
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
  fetchUsers();
};

const handleActionCompleted = () => {
  fetchUsers();
};

onMounted(() => {
  fetchUsers();
});
</script>