<template>
  <v-card v-if="isOpen" elevation="2">
    <v-toolbar>
      <v-toolbar-title class="text-truncate" style="max-width: 100px;">Salidas</v-toolbar-title>
      <v-divider class="mx-2" inset vertical></v-divider>
      <div class="font-weight-bold" style="font-size: 16px;">{{ localIssue.code }} </div>
      <v-spacer></v-spacer>
    </v-toolbar>
    <v-card-text>
      <v-form ref="formRef" v-model="valid">
        <v-row>
          <v-col cols="12" md="2">
            <v-select v-if="!localIssue.idIssue" color="indigo" variant="underlined" v-model="localIssue.type"
              :items="issueTypes" label="Tipo de Salida" :rules="[rules.required]" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localIssue.type" label="Tipo de Salida"
              readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-autocomplete v-if="!localIssue.idIssue" color="indigo" variant="underlined" :items="usersArray"
              v-model="localIssue.idUser" item-title="userName" item-value="idUser"
              :rules="[rules.required]" no-data-text="No hay datos disponibles" label="Personal"
              :loading="loadingUsers" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localIssue.userName"
              label="Personal" readonly />
          </v-col>
          <v-col class="px-2" cols="12" md="2">
            <v-btn v-if="!localIssue.idIssue" fab dark color="indigo" class="mt-3" @click="openProductModal">
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
            <td v-if="!localIssue.idIssue">
              <v-text-field v-model.number="item.quantity" variant="underlined" type="number" min="0"
                :rules="[rules.required, rules.minValue]"></v-text-field>
            </td>
            <td v-else>{{ item.quantity }}</td>
            <td v-if="!localIssue.idIssue">
              <v-text-field v-model.number="item.unitPrice" variant="underlined" type="number" min="0"
                :rules="localIssue.type === 'REGULARIZACIÓN' ? [rules.required, rules.minValueOrZero] : [rules.required, rules.minValue]"></v-text-field>
            </td>
            <td v-else>{{ formatCurrency(item.unitPrice) }}</td>
            <td v-if="!localIssue.idIssue">{{ formatCurrency(item.quantity * item.unitPrice) }}</td>
            <td v-else>{{ formatCurrency(item.totalPrice) }}</td>
            <td v-if="!localIssue.idIssue" class="text-center">
              <v-btn color="red" icon="delete" variant="text" @click="removeProduct(item)" size="small"
                title="Quitar" />
            </td>
          </tr>
        </template>
      </v-data-table>
      <v-col v-if="!localIssue.idIssue" cols="12" class="d-flex justify-end">
        <strong>Total Bs.</strong>{{ formatCurrency(totalPrice) }}
      </v-col>
     <v-col v-else cols="12" class="d-flex justify-end">
        <strong>Total Bs.</strong>{{ formatCurrency(localIssue.totalAmount) }}
      </v-col>
      <v-col cols="12" md="12" lg="12" xl="12">
        <v-text-field color="indigo" variant="underlined" label="Observaciones" counter="80" :maxlength="80"
          v-model="localIssue.annotations" :readonly="!!localIssue.idIssue"></v-text-field>
      </v-col>
    </v-card-text>
    <v-card-actions>
      <v-btn v-if="!localIssue.idIssue" color="green" dark class="mb-2" elevation="4" @click="saveIssue"
        :disabled="!valid || details.length === 0" :loading="saving">
        Guardar
      </v-btn>
      <v-btn v-else-if="localIssue.statusIssue === 'Activo' && canDownload" color="indigo" dark class="mb-2" elevation="4"
        @click="downloadPdf" :loading="downloading">
        Descargar
      </v-btn>
      <v-btn color="red" dark class="mb-2" elevation="4" @click="close">
        {{ localIssue.idIssue ? 'Cerrar' : 'Cancelar' }}
      </v-btn>
    </v-card-actions>
    <CommonProductOut v-model="productModal" @close="productModal = false" @product-added="handleProductAdded" />
  </v-card>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { useToast } from 'vue-toastification';
import { useGoodsIssueStore } from '@/stores/goodsIssueStore';
import { useUserStore } from '@/stores/userStore';
import { useAuthStore } from '@/stores/auth';
import { handleApiError } from '@/helpers/errorHandler';
import CommonProductOut from '@/components/Common/CommonProductOut.vue';
import { formatCurrency } from '@/utils/currency';
import { GoodsIssue, GoodsIssueDetail } from '@/interfaces/goodsIssueInterface';

interface FormRef {
  validate: () => boolean;
}

interface Props {
  modelValue: boolean;
  issue?: GoodsIssue | null;
  issueDetails?: GoodsIssueDetail[];
}

const props = withDefaults(defineProps<Props>(), {
  issue: () => ({
    idIssue: null,
    code: '',
    type: '',
    storeName: '',
    idUser: null,
    userName: '',
    totalAmount: 0,
    annotations: '',
    auditCreateDate: '',
    statusIssue: ''
  } as GoodsIssue),
  issueDetails: () => []
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [];
  'close': [];
}>();

const goodsIssueStore = useGoodsIssueStore();
const userStore = useUserStore();
const authStore = useAuthStore();
const toast = useToast();

const canDownload = computed(() => authStore.hasPermission('salida de productos', 'descargar'));

const { users, loading: loadingUsers } = storeToRefs(userStore);

const formRef = ref<FormRef | null>(null);
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const downloading = ref(false);
const productModal = ref(false);
const localIssue = ref<GoodsIssue>({ ...props.issue } as GoodsIssue);
const details = ref<GoodsIssueDetail[]>([]);
const documentTypes = ref<string[]>([]);

const issueTypes = ['CONSIGNACIÓN', 'REGULARIZACIÓN'];

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

  if (!localIssue.value.idIssue) {
    baseHeaders.push({ title: 'Acciones', key: 'actions', sortable: false, align: 'center' });
  }

  return baseHeaders;
});

const totalPrice = computed(() => {
  return details.value.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0);
});

const usersArray = computed(() => Array.isArray(users.value) ? users.value : []);

watch(() => props.modelValue, (newValue) => {
  isOpen.value = newValue;
});

watch(isOpen, (newValue) => {
  emit('update:modelValue', newValue);
});

watch(() => props.issue, (newIssue) => {
  if (newIssue) {
    localIssue.value = { ...newIssue };
  }
  details.value = [...props.issueDetails];
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

const removeProduct = (product: GoodsIssueDetail) => {
  const index = details.value.findIndex(d => d.idProduct === product.idProduct);

  if (index !== -1) {
    details.value.splice(index, 1);
    toast.error(`Producto ${product.code} eliminado de la lista`);
  }
};

const saveIssue = async () => {
  if (!formRef.value?.validate()) {
    toast.warning('Por favor completa todos los campos requeridos');
    return;
  }

  const invalidProducts = details.value.filter(d => {
    if (localIssue.value.type === 'REGULARIZACIÓN') {
      return d.quantity <= 0 || d.unitPrice === null || d.unitPrice === undefined || d.unitPrice < 0;
    } else {
      return d.quantity <= 0 || d.unitPrice <= 0;
    }
  });

  if (invalidProducts.length > 0) {
    if (localIssue.value.type === 'REGULARIZACIÓN') {
      toast.warning('Todos los productos deben tener cantidad mayor a 0 y precio mayor o igual a 0');
    } else {
      toast.warning('Todos los productos deben tener cantidad y precio válidos mayores a 0');
    }
    return;
  }

  saving.value = true;

  try {
    const issueData = {
      type: localIssue.value.type,
      totalAmount: totalPrice.value,
      annotations: localIssue.value.annotations || '',
      idUser: localIssue.value.idUser,
      idStore: authStore.currentUser?.storeId,
      goodsIssueDetails: details.value.map((d, index) => ({
        item: index + 1,
        idProduct: d.idProduct,
        quantity: d.quantity,
        unitPrice: d.unitPrice,
        totalPrice: d.quantity * d.unitPrice
      }))
    };

    const result = await goodsIssueStore.registerGoodsIssue(issueData);

    if (result.isSuccess) {
      toast.success('Salida registrada con éxito');
      emit('saved');
      close();
    }
  } catch (error) {
    handleApiError(error, 'Error al registrar la salida');
  } finally {
    saving.value = false;
  }
};

const downloadPdf = async () => {
  if (!localIssue.value.idIssue) return;

  downloading.value = true;
  try {
    await goodsIssueStore.exportGoodsIssuePdf(localIssue.value.idIssue);
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

onMounted(() => {
  details.value = [...props.issueDetails];
  userStore.selectUser();
});
</script>