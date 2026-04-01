<template>
  <v-container fluid>
    <v-card elevation="2" class="mb-4">
      <v-toolbar>
        <v-toolbar-title> <v-avatar color="indigo" size="36" class="mr-3">
            <v-icon icon="mdi-clipboard-text" color="white" size="18"></v-icon>
          </v-avatar>Kardex del Producto</v-toolbar-title>
      </v-toolbar>
      <v-card-text>
        <v-row align="center" class="mb-0 pb-0">
          <v-col cols="12" md="2">
            <v-text-field v-model="selectedProduct.code" label="Código" variant="outlined" density="compact"
              color="indigo" readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-text-field v-model="selectedProduct.description" label="Descripción" variant="outlined" density="compact"
              color="indigo" readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field v-model="selectedProduct.brandName" label="Marca" variant="outlined" density="compact"
              color="indigo" readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field v-model="selectedProduct.categoryName" label="Categoría" variant="outlined" density="compact"
              color="indigo" readonly />
          </v-col>
          <v-col cols="12" md="2" style="padding-bottom: 22px;">
            <v-tooltip v-bind="tooltipProps" text="Seleccionar Producto" location="bottom">
              <template v-slot:activator="{ props }">
                <v-btn v-bind="props" color="indigo" @click="productModal = true">
                  <v-icon icon="mdi-playlist-plus" size="24"></v-icon>
                </v-btn>
              </template>
            </v-tooltip>
          </v-col>
        </v-row>
        <v-row v-if="kardex" class="mt-0 mb-0">
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.color" label="Color" variant="outlined" density="compact" color="indigo"
              readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.material" label="Material" variant="outlined" density="compact"
              color="indigo" readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.unitMeasure" label="Unidad de Medida" variant="outlined"
              density="compact" color="indigo" readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.currentStock" label="Stock Actual" variant="outlined" density="compact"
              color="indigo" readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.calculatedStock" label="Stock Calculado" variant="outlined"
              density="compact" color="indigo" readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.stockDifference" label="Diferencia" variant="outlined" density="compact"
              :color="kardex.stockDifference !== 0 ? 'red' : 'green'" readonly />
          </v-col>
        </v-row>
        <v-row align="center" class="mt-0">
          <v-col cols="12" md="3">
            <v-date-input v-model="filters.startDate" label="Fecha Inicio" color="indigo" prepend-icon=""
              variant="outlined" density="compact" persistent-placeholder clearable
              :error-messages="dateError ? ' ' : ''" @update:model-value="validateDates" />
          </v-col>
          <v-col cols="12" md="3">
            <v-date-input v-model="filters.endDate" label="Fecha Fin" color="indigo" prepend-icon="" variant="outlined"
              density="compact" persistent-placeholder clearable :error-messages="dateError"
              @update:model-value="validateDates" />
          </v-col>
          <v-col cols="12" md="3" style="padding-bottom: 22px;">
            <v-tooltip v-bind="tooltipProps" text="Generar" location="bottom">
              <template v-slot:activator="{ props }">
                <v-btn v-bind="props" color="indigo" dark elevation="4" :disabled="!isEnabled" :loading="loading"
                  @click="generateKardex">
                  <v-icon icon="mdi-sync" size="24"></v-icon>
                </v-btn>
              </template>
            </v-tooltip>
            <template v-if="canDownload && kardex">
              <v-tooltip v-bind="tooltipProps" text="Descargar PDF" location="bottom">
                <template v-slot:activator="{ props }">
                  <v-btn v-bind="props" color="red" dark elevation="4" class="ml-2" :loading="downloadingPdf"
                    :disabled="downloadingPdf" @click="downloadPdf">
                    <v-icon icon="mdi-file-pdf-box" size="24"></v-icon>
                  </v-btn>
                </template>
              </v-tooltip>
              <v-tooltip v-bind="tooltipProps" text="Descargar Excel" location="bottom">
                <template v-slot:activator="{ props }">
                  <v-btn v-bind="props" color="green" dark elevation="4" class="ml-2" :loading="downloadingExcel"
                    :disabled="downloadingExcel" @click="downloadExcel">
                    <v-icon icon="mdi-file-excel-box" size="24"></v-icon>
                  </v-btn>
                </template>
              </v-tooltip>
            </template>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>
    <KardexTable :movements="kardex?.movements ?? []" :total-movements="totalKardex" :loading="loading"
      :items-per-page="filters.pageSize" @update-items-per-page="handleItemsPerPage" @change-page="handleChangePage" />
    <CommonProductOut v-model="productModal" @close="productModal = false" @product-added="handleProductSelected" />
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onUnmounted } from 'vue';
import { storeToRefs } from 'pinia';
import { useToast } from 'vue-toastification';
import { useKardexStore } from '@/stores/kardexStore';
import { useAuthStore } from '@/stores/auth';
import { handleApiError } from '@/helpers/errorHandler';
import KardexTable from '@/components/Kardex/KardexList.vue';
import CommonProductOut from '@/components/Common/CommonProductOut.vue';
import { FilterParams } from '@/interfaces/baseInterface';
import { useResponsiveTooltip } from '@/composables/useResponsiveTooltip';

const kardexStore = useKardexStore();
const authStore = useAuthStore();
const toast = useToast();

const { kardex, loading, totalKardex } = storeToRefs(kardexStore);
const { tooltipProps } = useResponsiveTooltip();
const productModal = ref(false);
const downloadingExcel = ref(false);
const downloadingPdf = ref(false);
const dateError = ref<string>('');

const selectedProduct = ref({
  idProduct: null as number | null,
  code: '',
  description: '',
  brandName: '',
  categoryName: '',
});

const filters = ref<FilterParams>({
  pageNumber: 1,
  pageSize: 10,
  order: 'asc',
  sort: 'Id',
  startDate: '',
  endDate: '',
});

const canDownload = computed((): boolean => authStore.hasPermission('inventario', 'descargar'));

const validateDates = () => {
  const start = filters.value.startDate;
  const end = filters.value.endDate;
  if (start && end && new Date(start) > new Date(end)) {
    dateError.value = 'La fecha fin no puede ser menor a la fecha inicio';
  } else {
    dateError.value = '';
  }
};

const isEnabled = computed(() => {
  const hasProduct = selectedProduct.value.idProduct !== null;
  const hasStartDate = filters.value.startDate && filters.value.startDate !== '' && filters.value.startDate !== null;
  const hasEndDate = filters.value.endDate && filters.value.endDate !== '' && filters.value.endDate !== null;
  return hasProduct && hasStartDate && hasEndDate && !dateError.value;
});

const handleProductSelected = (product: any) => {
  kardexStore.clearKardex();
  filters.value.startDate = '';
  filters.value.endDate = '';
  filters.value.pageNumber = 1;
  dateError.value = '';

  selectedProduct.value = {
    idProduct: product.idProduct,
    code: product.code,
    description: product.description,
    brandName: product.brandName,
    categoryName: product.categoryName,
  };
  productModal.value = false;
  toast.success(`Producto ${product.code} seleccionado`);
};

const generateKardex = async () => {
  if (!selectedProduct.value.idProduct) {
    toast.warning('Seleccione un producto primero');
    return;
  }
  try {
    await kardexStore.fetchKardex(selectedProduct.value.idProduct, {
      ...filters.value,
      startDate: filters.value.startDate || undefined,
      endDate: filters.value.endDate || undefined,
    });
  } catch (error) {
    handleApiError(error, 'Error al generar el kardex');
  }
};

const downloadExcel = async () => {
  downloadingExcel.value = true;
  try {
    await kardexStore.downloadKardexExcel({
      ...filters.value,
      startDate: filters.value.startDate || undefined,
      endDate: filters.value.endDate || undefined,
    });
    toast.success('Archivo descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo Excel');
  } finally {
    downloadingExcel.value = false;
  }
};

const downloadPdf = async () => {
  downloadingPdf.value = true;
  try {
    await kardexStore.downloadKardexPdf({
      ...filters.value,
      startDate: filters.value.startDate || undefined,
      endDate: filters.value.endDate || undefined,
    });
    toast.success('Archivo PDF descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo PDF');
  } finally {
    downloadingPdf.value = false;
  }
};

const handleItemsPerPage = (value: number) => {
  filters.value.pageSize = value;
  filters.value.pageNumber = 1;
  generateKardex();
};

const handleChangePage = (page: number) => {
  filters.value.pageNumber = page;
  generateKardex();
};

onUnmounted(() => {
  kardexStore.clearKardex();
  selectedProduct.value = {
    idProduct: null,
    code: '',
    description: '',
    brandName: '',
    categoryName: '',
  };
});
</script>