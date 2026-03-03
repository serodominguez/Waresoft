<template>
  <v-container fluid>
    <v-card elevation="2" class="mb-4">
      <v-toolbar>
        <v-toolbar-title>Kardex del Producto</v-toolbar-title>
      </v-toolbar>
      <v-card-text>
        <v-row align="center">
          <v-col cols="12" md="2">
            <v-text-field v-model="selectedProduct.code" label="Código" variant="underlined" color="indigo" readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-text-field v-model="selectedProduct.description" label="Descripción" variant="underlined" color="indigo"
              readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field v-model="selectedProduct.brandName" label="Marca" variant="underlined" color="indigo"
              readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field v-model="selectedProduct.categoryName" label="Categoría" variant="underlined" color="indigo"
              readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-btn color="indigo" class="mt-1" @click="productModal = true">
              <v-icon>list</v-icon>
            </v-btn>
          </v-col>
        </v-row>
        <v-row v-if="kardex">
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.color" label="Color" variant="underlined" color="indigo" readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.material" label="Material" variant="underlined" color="indigo"
              readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.unitMeasure" label="Unidad de Medida" variant="underlined" color="indigo"
              readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.currentStock" label="Stock Actual" variant="underlined" color="indigo"
              readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.calculatedStock" label="Stock Calculado" variant="underlined"
              color="indigo" readonly />
          </v-col>
          <v-col cols="12" md="1">
            <v-text-field :model-value="kardex.stockDifference" label="Diferencia" variant="underlined"
              :color="kardex.stockDifference !== 0 ? 'red' : 'green'" readonly />
          </v-col>
        </v-row>
        <v-row align="center">
          <v-col cols="12" md="3">
            <v-date-input v-model="filters.startDate" label="Fecha Inicio" prepend-icon="" variant="underlined"
              persistent-placeholder clearable ></v-date-input>
          </v-col>
          <v-col cols="12" md="3">
            <v-date-input v-model="filters.endDate" label="Fecha Fin" prepend-icon="" variant="underlined"
              persistent-placeholder clearable ></v-date-input>
          </v-col>
          <v-col cols="12" md="2" class="d-flex align-center">
            <v-btn color="indigo" dark elevation="4" :disabled="!selectedProduct.idProduct" :loading="loading"
              @click="generateKardex">
              Generar
            </v-btn>
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
import { ref, onUnmounted } from 'vue';
import { storeToRefs } from 'pinia';
import { useToast } from 'vue-toastification';
import { useKardexStore } from '@/stores/kardexStore';
import { handleApiError } from '@/helpers/errorHandler';
import KardexTable from '@/components/Kardex/KardexList.vue';
import CommonProductOut from '@/components/Common/CommonProductOut.vue';
import { FilterParams } from '@/interfaces/baseInterface';

const kardexStore = useKardexStore();
const toast = useToast();

const { kardex, loading, totalKardex } = storeToRefs(kardexStore);

const productModal = ref(false);

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

const handleProductSelected = (product: any) => {
  kardexStore.clearKardex();
  filters.value.startDate  = '';
  filters.value.endDate    = '';
  filters.value.pageNumber = 1;

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
    idProduct:    null,
    code:         '',
    description:  '',
    brandName:    '',
    categoryName: '',
  };
});
</script>