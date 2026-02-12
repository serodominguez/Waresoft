<template>
  <v-card v-if="isOpen" elevation="2">
    <v-toolbar>
      <v-toolbar-title class="text-truncate" style="max-width: 100px;">Traspasos</v-toolbar-title>
      <v-divider class="mx-2" inset vertical></v-divider>
      <div class="font-weight-bold" style="font-size: 16px;">{{ localTransfer.code }} </div>
      <v-spacer></v-spacer>
    </v-toolbar>
    <v-card-text>
      <v-form ref="formRef" v-model="valid">
        <v-row>
          <v-col cols="12" md="2">
            <v-autocomplete v-if="!localTransfer.idTransfer" color="indigo" variant="underlined" :items="storesArray"
              v-model="localTransfer.idStoreDestination" item-title="storeName" item-value="idStore"
              :rules="[rules.required]" no-data-text="No hay datos disponibles" label="Establecimiento"
              :loading="loadingStores" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localTransfer.storeDestination"
              label="Establecimiento" readonly />
          </v-col>
          <v-col class="px-2" cols="12" md="2">
            <v-btn v-if="!localTransfer.idTransfer" fab dark color="indigo" class="mt-3" @click="openProductModal">
              <v-icon dark>list</v-icon>
            </v-btn>
          </v-col>
        </v-row>
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
            <td v-if="!localTransfer.idTransfer">
              <v-text-field v-model.number="item.quantity" variant="underlined" type="number" min="0"
                :rules="[rules.required, rules.minValue]"></v-text-field>
            </td>
            <td v-else>{{ item.quantity }}</td>
            <td v-if="!localTransfer.idTransfer">
              <v-text-field v-model.number="item.unitPrice" variant="underlined" type="number" min="0"
                :rules="[rules.required, rules.minValueOrZero]"></v-text-field>
            </td>
            <td v-else>{{ formatCurrency(item.unitPrice) }}</td>
            <td v-if="!localTransfer.idTransfer">{{ formatCurrency(item.quantity * item.unitPrice) }}</td>
            <td v-else>{{ formatCurrency(item.totalPrice) }}</td>
            <td v-if="!localTransfer.idTransfer" class="text-center">
              <v-btn color="red" icon="delete" variant="text" @click="removeProduct(item)" size="small"
                title="Quitar" />
            </td>
          </tr>
        </template>
      </v-data-table>
      <v-col v-if="!localTransfer.idTransfer" cols="12" class="d-flex justify-end">
        <strong>Total Bs.</strong>{{ formatCurrency(totalPrice) }}
      </v-col>
     <v-col v-else cols="12" class="d-flex justify-end">
        <strong>Total Bs.</strong>{{ formatCurrency(localTransfer.totalAmount) }}
      </v-col>
      <v-col cols="12" md="12" lg="12" xl="12">
        <v-text-field color="indigo" variant="underlined" label="Observaciones" counter="80" :maxlength="80"
          v-model="localTransfer.annotations" :readonly="!!localTransfer.idTransfer"></v-text-field>
      </v-col>
    </v-card-text>
    <v-card-actions>
      <v-btn v-if="!localTransfer.idTransfer" color="green" dark class="mb-2" elevation="4" @click="saveTransfer"
        :disabled="!valid || details.length === 0" :loading="saving">
        Enviar
      </v-btn>
      <v-btn v-if="localTransfer.statusTransfer === 'Pendiente'" color="indigo" dark class="mb-2" elevation="4"
        @click="receiveTransfer" :loading="downloading">
        Recibir
      </v-btn>
      <v-btn color="red" dark class="mb-2" elevation="4" @click="close">
        {{ localTransfer.idTransfer ? 'Cerrar' : 'Cancelar' }}
      </v-btn>
    </v-card-actions>
    <CommonProductOut v-model="productModal" @close="productModal = false" @product-added="handleProductAdded" />
  </v-card>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { useToast } from 'vue-toastification';
import { useTransferStore } from '@/stores/transferStore';
import { useStoreStore } from '@/stores/storeStore';
import { useAuthStore } from '@/stores/auth';
import { handleApiError } from '@/helpers/errorHandler';
import CommonProductOut from '@/components/Common/CommonProductOut.vue';
import { formatCurrency } from '@/utils/currency';
import { Transfer, TransferDetail } from '@/interfaces/transferInterface';

interface FormRef {
  validate: () => boolean;
}

interface Props {
  modelValue: boolean;
  transfer?: Transfer | null;
  transferDetails?: TransferDetail[];
}

const props = withDefaults(defineProps<Props>(), {
  transfer: () => ({
    idTransfer: null,
    code: '',
    storeOrigin: '',
    idStoreOrigin: null,
    idStoreDestination: null,
    storeDestination: '',
    totalAmount: 0,
    annotations: '',
    userName: '',
    statusTransfer: ''
  } as Transfer),
  transferDetails: () => []
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [];
  'close': [];
}>();

const transferStore = useTransferStore();
const storeStore = useStoreStore();
const authStore = useAuthStore();
const toast = useToast();

const canDownload = computed(() => authStore.hasPermission('traspaso de productos', 'descargar'));

const { stores, loading: loadingStores } = storeToRefs(storeStore);

const formRef = ref<FormRef | null>(null);
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const downloading = ref(false);
const productModal = ref(false);
const localTransfer = ref<Transfer>({ ...props.transfer } as Transfer);
const details = ref<TransferDetail[]>([]);

const rules = {
  required: (value: any) => !!value || 'Este campo es requerido',
  minValue: (value: number) => value > 0 || 'Debe ser mayor a 0',
  minValueOrZero: (value: number) => (value !== null && value !== undefined && value >= 0) || 'Debe ser mayor o igual a 0'
};

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
    { title: 'Precio U.', key: 'price', sortable: false },
    { title: 'SubTotal', key: 'subtotal', sortable: false }
  ];

  if (!localTransfer.value.idTransfer) {
    baseHeaders.push({ title: 'Acciones', key: 'actions', sortable: false, align: 'center' });
  }

  return baseHeaders;
});

const totalPrice = computed(() => {
  return details.value.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0);
});

const storesArray = computed(() => Array.isArray(stores.value) ? stores.value : []);

watch(() => props.modelValue, (newValue) => {
  isOpen.value = newValue;
});

watch(isOpen, (newValue) => {
  emit('update:modelValue', newValue);
});

watch(() => props.transfer, (newTransfer) => {
  if (newTransfer) {
    localTransfer.value = { ...newTransfer };
  }
  details.value = [...props.transferDetails];
}, { deep: true });

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
    unitPrice: product.price,
    totalPrice: 0
  });

  toast.success('Producto agregado a la lista');
};

const removeProduct = (product: TransferDetail) => {
  const index = details.value.findIndex(d => d.idProduct === product.idProduct);

  if (index !== -1) {
    details.value.splice(index, 1);
    toast.error(`Producto ${product.code} eliminado de la lista`);
  }
};

const saveTransfer = async () => {
  if (!formRef.value?.validate()) {
    toast.warning('Por favor completa todos los campos requeridos');
    return;
  }

  saving.value = true;

  try {
    const transferData = {
      totalAmount: totalPrice.value,
      annotations: localTransfer.value.annotations || '',
      idStoreDestination: localTransfer.value.idStoreDestination,
      idStoreOrigin: authStore.currentUser?.storeId,
      transferDetails: details.value.map((d, index) => ({
        item: index + 1,
        idProduct: d.idProduct,
        quantity: d.quantity,
        unitPrice: d.unitPrice,
        totalPrice: d.quantity * d.unitPrice
      }))
    };

    const result = await transferStore.sendTrasnfer(transferData);

    if (result.isSuccess) {
      toast.success('Traspaso registrada con éxito');
      emit('saved');
      close();
    }
  } catch (error) {
    handleApiError(error, 'Error al registrar el traspaso');
  } finally {
    saving.value = false;
  }
};

const receiveTransfer = async () => {
  if (!localTransfer.value.idTransfer) {
    toast.warning('No se puede recibir un traspaso sin ID');
    return;
  }

  downloading.value = true;

  try {
    const result = await transferStore.receiveTransfer(localTransfer.value.idTransfer);

    if (result.isSuccess) {
      toast.success('Traspaso recibido con éxito');
      emit('saved');
      close();
    } else {
      toast.error(result.message || 'Error al recibir el traspaso');
    }
  } catch (error) {
    handleApiError(error, 'Error al recibir el traspaso');
  } finally {
    downloading.value = false;
  }
};

const close = () => {
  isOpen.value = false;
  emit('close');
};

onMounted(() => {
  details.value = [...props.transferDetails];
  storeStore.selectStore();
});
</script>