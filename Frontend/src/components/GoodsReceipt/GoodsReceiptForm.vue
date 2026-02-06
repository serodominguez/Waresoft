<template>
  <v-card v-if="isOpen" elevation="2">
    <v-toolbar>
      <v-toolbar-title class="text-truncate" style="max-width: 100px;">Entradas</v-toolbar-title>
      <v-divider class="mx-2" inset vertical></v-divider>
      <div class="font-weight-bold" style="font-size: 16px;">{{ localReceipt.code }} </div>
      <v-spacer></v-spacer>
    </v-toolbar>
    <v-card-text>
      <v-form ref="formRef" v-model="valid">
      <v-container class="px-4" max-width="1700px">
        <v-row justify="center">
          <v-col cols="12" md="2">
            <v-select v-if="!localReceipt.idReceipt" color="indigo" variant="underlined" v-model="localReceipt.type"
              :items="receiptTypes" label="Tipo de Entrada" :rules="[rules.required]"
              @update:modelValue="updateDocuments" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localReceipt.type" label="Tipo de Entrada"
              readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-select v-if="!localReceipt.idReceipt" color="indigo" variant="underlined"
              v-model="localReceipt.documentType" :items="documentTypes" label="Tipo de Comprobante"
              :rules="[rules.required]" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localReceipt.documentType"
              label="Tipo de Comprobante" readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-text-field v-if="!localReceipt.idReceipt" color="indigo" variant="underlined"
              v-model="localReceipt.documentNumber" :rules="documentNumberRules" counter="30" :maxlength="30"
              label="Número del Comprobante" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localReceipt.documentNumber"
              label="Número del Comprobante" readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-date-input v-if="!localReceipt.idReceipt" locale="es" placeholder="dd/mm/yyyy"
              v-model="localReceipt.documentDate" label="Fecha del Comprobante" variant="underlined" prepend-icon=""
              :rules="[rules.required]" />
            <v-text-field v-else v-model="localReceipt.documentDate" label="Fecha del Comprobante" variant="underlined"
              readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-autocomplete v-if="!localReceipt.idReceipt" color="indigo" variant="underlined" :items="suppliersArray"
              v-model="localReceipt.idSupplier" item-title="companyName" item-value="idSupplier"
              :rules="[rules.required]" no-data-text="No hay datos disponibles" label="Proveedor"
              :loading="loadingSuppliers" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localReceipt.companyName"
              label="Proveedor" readonly />
          </v-col>
          <v-col class="px-2" cols="12" md="2">
            <v-btn v-if="!localReceipt.idReceipt" fab dark color="indigo" class="mt-3" @click="openProductModal">
              <v-icon dark>list</v-icon>
            </v-btn>
          </v-col>
        </v-row>
        </v-container>
      </v-form>
      <v-divider class="my-4"></v-divider>
      <v-data-table :headers="headers" :items="details" class="elevation-1" hide-default-footer
        :no-data-text="'No hay productos agregados'">
        <template v-slot:item="{ item, index }">
          <tr>
            <td class="text-center">{{ index + 1 }}</td>
            <td>{{ item.code }}</td>
            <td>{{ item.description }}</td>
            <td>{{ item.material }}</td>
            <td>{{ item.color }}</td>
            <td>{{ item.categoryName }}</td>
            <td>{{ item.brandName }}</td>
            <td v-if="!localReceipt.idReceipt">
              <v-text-field v-model.number="item.quantity" variant="underlined" type="number" min="0"
                :rules="[rules.required, rules.minValue]"></v-text-field>
            </td>
            <td v-else>{{ item.quantity }}</td>
            <td v-if="!localReceipt.idReceipt">
              <v-text-field v-model.number="item.unitCost" variant="underlined" type="number" min="0"
                :rules="localReceipt.type === 'REGULARIZACIÓN' ? [rules.required, rules.minValueOrZero] : [rules.required, rules.minValue]"></v-text-field>
            </td>
            <td v-else>{{ formatCurrency(item.unitCost) }}</td>
            <td v-if="!localReceipt.idReceipt">{{ formatCurrency(item.quantity * item.unitCost) }}</td>
            <td v-else>{{ formatCurrency(item.totalCost) }}</td>
            <td v-if="!localReceipt.idReceipt" class="text-center">
              <v-btn color="red" icon="delete" variant="text" @click="removeProduct(item)" size="small"
                title="Quitar" />
            </td>
          </tr>
        </template>
      </v-data-table>
      <v-col v-if="!localReceipt.idReceipt" cols="12" class="d-flex justify-end">
        <strong>Total Bs.</strong>{{ formatCurrency(totalCost) }}
      </v-col>
     <v-col v-else cols="12" class="d-flex justify-end">
        <strong>Total Bs.</strong>{{ formatCurrency(localReceipt.totalAmount) }}
      </v-col>
      <v-col cols="12" md="12" lg="12" xl="12">
        <v-text-field color="indigo" variant="underlined" label="Observaciones" counter="80" :maxlength="80"
          v-model="localReceipt.annotations" :readonly="!!localReceipt.idReceipt"></v-text-field>
      </v-col>
    </v-card-text>
    <v-card-actions>
      <v-btn v-if="!localReceipt.idReceipt" color="green" dark class="mb-2" elevation="4" @click="saveReceipt"
        :disabled="!valid || details.length === 0" :loading="saving">
        Guardar
      </v-btn>
      <v-btn v-else-if="localReceipt.statusReceipt === 'Activo' && canDownload" color="indigo" dark class="mb-2" elevation="4"
        @click="downloadPdf" :loading="downloading">
        Descargar
      </v-btn>
      <v-btn color="red" dark class="mb-2" elevation="4" @click="close">
        {{ localReceipt.idReceipt ? 'Cerrar' : 'Cancelar' }}
      </v-btn>
    </v-card-actions>
    <CommonProductIn v-model="productModal" @close="productModal = false" @product-added="handleProductAdded" />
  </v-card>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { useToast } from 'vue-toastification';
import { useGoodsReceiptStore } from '@/stores/goodsReceiptStore';
import { useSupplierStore } from '@/stores/supplierStore';
import { useAuthStore } from '@/stores/auth';
import { handleApiError } from '@/helpers/errorHandler';
import CommonProductIn from '@/components/Common/CommonProductIn.vue';
import { formatDateForApi } from '@/utils/date';
import { formatCurrency } from '@/utils/currency';
import { GoodsReceipt, GoodsReceiptDetail } from '@/interfaces/goodsReceiptInterface';

interface FormRef {
  validate: () => boolean;
}

interface Props {
  modelValue: boolean;
  receipt?: GoodsReceipt | null;
  receiptDetails?: GoodsReceiptDetail[];
}

const props = withDefaults(defineProps<Props>(), {
  receipt: () => ({
    idReceipt: null,
    code: '',
    type: '',
    storeName: '',
    documentType: '',
    documentNumber: '',
    documentDate: '',
    idSupplier: null,
    companyName: '',
    totalAmount: 0,
    annotations: '',
    auditCreateDate: '',
    statusReceipt: ''
  } as GoodsReceipt),
  receiptDetails: () => []
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [];
  'close': [];
}>();

const goodsReceiptStore = useGoodsReceiptStore();
const supplierStore = useSupplierStore();
const authStore = useAuthStore();
const toast = useToast();

const canDownload = computed(() => authStore.hasPermission('entrada de productos', 'descargar'));

const { suppliers, loading: loadingSuppliers } = storeToRefs(supplierStore);

// Refs
const formRef = ref<FormRef | null>(null);
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const downloading = ref(false);
const productModal = ref(false);
const localReceipt = ref<GoodsReceipt>({ ...props.receipt } as GoodsReceipt);
const details = ref<GoodsReceiptDetail[]>([]);
const documentTypes = ref<string[]>([]);

// Constants
const receiptTypes = ['ADQUISICIÓN', 'REGULARIZACIÓN'];
const typesPurchases = ['FACTURA', 'RECIBO'];
const typesImports = ['ENTRADA'];

const rules = {
  required: (value: any) => !!value || 'Este campo es requerido',
  minValue: (value: number) => value > 0 || 'Debe ser mayor a 0',
  minValueOrZero: (value: number) => (value !== null && value !== undefined && value >= 0) || 'Debe ser mayor o igual a 0'
};

// Computed
const headers = computed(() => {
  const baseHeaders: Array<{ title: string; key: string; sortable: boolean; align?: 'start' | 'end' | 'center' }> = [
    { title: 'Item', key: 'item', sortable: false, align: 'center' },
    { title: 'Código', key: 'code', sortable: false },
    { title: 'Descripción', key: 'description', sortable: false },
    { title: 'Material', key: 'material', sortable: false },
    { title: 'Color', key: 'color', sortable: false },
    { title: 'Categoría', key: 'categoryName', sortable: false },
    { title: 'Marca', key: 'brandName', sortable: false },
    { title: 'Cantidad', key: 'quantity', sortable: false },
    { title: 'Costo U.', key: 'cost', sortable: false },
    { title: 'SubTotal', key: 'subtotal', sortable: false }
  ];

  if (!localReceipt.value.idReceipt) {
    baseHeaders.push({ title: 'Acciones', key: 'actions', sortable: false, align: 'center' });
  }

  return baseHeaders;
});

const documentNumberRules = computed(() => {
  if (localReceipt.value.type === 'ADQUISICIÓN') {
    return [rules.required];
  }
  return [];
});

const totalCost = computed(() => {
  return details.value.reduce((sum, item) => sum + (item.quantity * item.unitCost), 0);
});

const suppliersArray = computed(() => Array.isArray(suppliers.value) ? suppliers.value : []);

// Watchers
watch(() => props.modelValue, (newValue) => {
  isOpen.value = newValue;
});

watch(isOpen, (newValue) => {
  emit('update:modelValue', newValue);
});

watch(() => props.receipt, (newReceipt) => {
  if (newReceipt) {
    localReceipt.value = { ...newReceipt };
  }
  details.value = [...props.receiptDetails];
  updateDocuments();
}, { deep: true });

// Methods
const updateDocuments = () => {
  localReceipt.value.documentType = '';

  if (localReceipt.value.type === 'ADQUISICIÓN') {
    documentTypes.value = typesPurchases;
  } else if (localReceipt.value.type === 'REGULARIZACIÓN') {
    documentTypes.value = typesImports;
  } else {
    documentTypes.value = [];
  }
};

const openProductModal = () => {
  productModal.value = true;
};

const handleProductAdded = (product: any) => {
  const exists = details.value.find(d => d.idProduct === product.idProduct);

  if (exists) {
    toast.warning('Este producto ya está en la lista');
    return;
  }

  details.value.push({
    idProduct: product.idProduct,
    code: product.code,
    description: product.description,
    material: product.material,
    color: product.color,
    categoryName: product.categoryName,
    brandName: product.brandName,
    quantity: 1,
    unitCost: 0,
    totalCost: 0
  });

  toast.success('Producto agregado a la lista');
};

const removeProduct = (product: GoodsReceiptDetail) => {
  const index = details.value.findIndex(d => d.idProduct === product.idProduct);

  if (index !== -1) {
    details.value.splice(index, 1);
    toast.error(`Producto ${product.code} eliminado de la lista`);
  }
};

const saveReceipt = async () => {
  if (!formRef.value?.validate()) {
    toast.warning('Por favor completa todos los campos requeridos');
    return;
  }

  const invalidProducts = details.value.filter(d => {
    if (localReceipt.value.type === 'REGULARIZACIÓN') {
      return d.quantity <= 0 || d.unitCost === null || d.unitCost === undefined || d.unitCost < 0;
    } else {
      return d.quantity <= 0 || d.unitCost <= 0;
    }
  });

  if (invalidProducts.length > 0) {
    if (localReceipt.value.type === 'REGULARIZACIÓN') {
      toast.warning('Todos los productos deben tener cantidad mayor a 0 y costo mayor o igual a 0');
    } else {
      toast.warning('Todos los productos deben tener cantidad y costo válidos mayores a 0');
    }
    return;
  }

  saving.value = true;

  try {
    const receiptData = {
      type: localReceipt.value.type,
      documentDate: formatDateForApi(localReceipt.value.documentDate),
      documentType: localReceipt.value.documentType,
      documentNumber: localReceipt.value.documentNumber,
      totalAmount: totalCost.value,
      annotations: localReceipt.value.annotations || '',
      idSupplier: localReceipt.value.idSupplier,
      idStore: authStore.currentUser?.storeId,
      goodsReceiptDetails: details.value.map((d, index) => ({
        item: index + 1,
        idProduct: d.idProduct,
        quantity: d.quantity,
        unitCost: d.unitCost,
        totalCost: d.quantity * d.unitCost
      }))
    };

    const result = await goodsReceiptStore.registerGoodsReceipt(receiptData);

    if (result.isSuccess) {
      toast.success('Entrada registrada con éxito');
      emit('saved');
      close();
    }
  } catch (error) {
    handleApiError(error, 'Error al registrar la entrada');
  } finally {
    saving.value = false;
  }
};

const downloadPdf = async () => {
  if (!localReceipt.value.idReceipt) return;

  downloading.value = true;
  try {
    await goodsReceiptStore.exportGoodsReceiptPdf(localReceipt.value.idReceipt);
    toast.success('PDF descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el PDF');
  } finally {
    downloading.value = false;
  }
};

const close = () => {
  isOpen.value = false;
  emit('close');
};

// Lifecycle
onMounted(() => {
  details.value = [...props.receiptDetails];
  supplierStore.selectSupplier();
  if (!localReceipt.value.idReceipt) {
    localReceipt.value.documentDate = '';
    updateDocuments();
  }
});
</script>
