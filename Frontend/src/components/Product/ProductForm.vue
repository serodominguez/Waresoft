<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localProduct.idProduct ? 'Editar Producto' : 'Agregar Producto' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text class="pb-0">
        <v-form ref="formRef" v-model="valid">
          <v-container class="pa-0">
            <v-row density="compact">
              <v-col cols="12" md="12">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localProduct.code"
                  counter="25" :maxlength="25" label="Código" :loading="generatingCode">
                  <template v-slot:append-inner>
                    <v-tooltip v-bind="tooltipProps" text="Generar Código" location="bottom">
                      <template v-slot:activator="{ props }">
                        <v-icon v-bind="props" icon="mdi-barcode" @click="handleGenerateCode" style="cursor: pointer;">
                        </v-icon>
                      </template>
                    </v-tooltip>
                  </template>
                </v-text-field>
              </v-col>
              <v-col cols="12" md="12">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localProduct.description"
                  :rules="[rules.required]" counter="50" :maxlength="50" label="Descripción" required />
              </v-col>
              <v-col cols="12" md="12">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localProduct.unitMeasure"
                  counter="15" :rules="[rules.required]" :maxlength="15" label="Unidad de medida" required />
              </v-col>
              <v-col cols="6" md="6">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localProduct.material"
                  :rules="[rules.onlyLetters]" counter="25" :maxlength="25" label="Material" />
              </v-col>
              <v-col cols="6" md="6">
                <v-text-field color="indigo" variant="outlined" density="compact" v-model="localProduct.color"
                  :rules="[rules.onlyLetters]" counter="20" :maxlength="20" label="Color" />
              </v-col>
              <v-col cols="6" md="6">
                <v-autocomplete color="indigo" variant="outlined" density="compact" :items="brandsArray"
                  v-model="localProduct.idBrand" item-title="brandName" item-value="idBrand" :rules="[rules.required]"
                  no-data-text="No hay datos disponibles" label="Marca" required :loading="loadingBrands" />
              </v-col>
              <v-col cols="6" md="6">
                <v-autocomplete color="indigo" variant="outlined" density="compact" :items="categoriesArray"
                  v-model="localProduct.idCategory" item-title="categoryName" item-value="idCategory"
                  :rules="[rules.required]" no-data-text="No hay datos disponibles" label="Categoría" required
                  :loading="loadingCategories" />
              </v-col>
              <v-col cols="12" md="12" class="mb-0">
                <v-file-input color="indigo" variant="outlined" density="compact" label="Imagen"
                  accept="image/jpeg,image/png,image/webp" prepend-icon="mdi-image" :clearable="true"
                  :rules="[rules.imageSize]" @change="handleImageChange" />
              </v-col>
            </v-row>
            <v-row v-if="localProduct.image && !selectedImage" align="center" justify="center" class="mt-0">
              <v-col cols="auto">
                <v-img :src="localProduct.image" max-height="100" contain class="border rounded pa-2 elevation-2"
                  style="width: 250px;" />
              </v-col>
              <v-col cols="auto" class="d-flex align-center">
                <v-tooltip v-bind="tooltipProps" text="Eliminar Imagen" location="bottom">
                  <template v-slot:activator="{ props }">
                    <v-btn v-bind="props" icon variant="text" color="red" size="small" @click="removeCurrentImage">
                      <v-icon icon="mdi-minus-circle-outline" size="24"></v-icon>
                    </v-btn>
                  </template>
                </v-tooltip>
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-card-actions :class="['px-6', 'pb-4', (localProduct.image && !selectedImage) ? 'pt-4' : 'pt-2']">
        <v-btn color="green" dark elevation="4" @click="saveProduct" :disabled="!valid"
          :loading="saving">Guardar</v-btn>
        <v-btn color="red" dark elevation="4" @click="close">Cancelar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue';
import { storeToRefs } from 'pinia';
import { useToast } from 'vue-toastification';
import { useProductStore } from '@/stores/productStore';
import { useBrandStore } from '@/stores/brandStore';
import { useCategoryStore } from '@/stores/categoryStore';
import { Product } from '@/interfaces/productInterface';
import { handleApiError } from '@/helpers/errorHandler';
import { useResponsiveTooltip } from '@/composables/useResponsiveTooltip';

interface FormRef {
  validate: () => Promise<{ valid: boolean }>;
}

interface Props {
  modelValue: boolean;
  product?: Product | null;
}

const props = withDefaults(defineProps<Props>(), {
  product: () => ({
    idProduct: null,
    code: '',
    description: '',
    material: '',
    color: '',
    unitMeasure: '',
    image: '',
    idBrand: null,
    brandName: '',
    idCategory: null,
    categoryName: '',
    auditCreateDate: '',
    statusProduct: ''
  })
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [];
}>();

const productStore = useProductStore();
const brandStore = useBrandStore();
const categoryStore = useCategoryStore();
const toast = useToast();

const { list: brands, loading: loadingBrands } = storeToRefs(brandStore);
const { list: categories, loading: loadingCategories } = storeToRefs(categoryStore);
const { tooltipProps } = useResponsiveTooltip();

const formRef = ref<FormRef | null>(null);
const selectedImage = ref<File | null>(null);
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const imageDeleted = ref(false);
const generatingCode = ref(false);
const localProduct = ref<Product>({ ...props.product } as Product);

const rules = {
  required: (value: string) => !!value || 'Este campo es requerido.',
  onlyLetters: (value: string) => !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) || 'Solo se permiten letras.',
  onlyNumbers: (value: string) => !value || /^[0-9]+$/.test(value) || 'Solo se permiten números.',
  imageSize: (value: File | File[]) => {
    if (!value) return true;
    const file = Array.isArray(value) ? value[0] : value;
    if (!file) return true;
    const maxSize = 2 * 1024 * 1024;
    const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];
    if (!allowedTypes.includes(file.type)) return 'Solo se permiten imágenes jpg, jpeg, png o webp.';
    if (file.size > maxSize) return 'La imagen no puede superar los 2MB.';
    return true;
  }
};

const brandsArray = computed(() => Array.isArray(brands.value) ? brands.value : []);
const categoriesArray = computed(() => Array.isArray(categories.value) ? categories.value : []);

watch(() => props.modelValue, (newValue: boolean) => {
  isOpen.value = newValue;
  if (newValue) {
    brandStore.fetchForSelect();
    categoryStore.fetchForSelect();
    selectedImage.value = null;
    imageDeleted.value = false;
    localProduct.value = { ...props.product } as Product;
  }
});

watch(isOpen, (newValue: boolean) => {
  emit('update:modelValue', newValue);
});

watch(() => props.product, (newProduct) => {
  if (newProduct) {
    localProduct.value = { ...newProduct } as Product;
    selectedImage.value = null;
    imageDeleted.value = false;
  }
}, { deep: true });

const close = () => {
  isOpen.value = false;
  selectedImage.value = null;
  imageDeleted.value = false;
};

const saveProduct = async () => {
  if (!formRef.value) {
    toast.warning('Error al acceder al formulario');
    return;
  }

  const validation = await formRef.value.validate();

  if (!validation.valid) {
    toast.warning('Por favor completa todos los campos requeridos');
    return;
  }

  saving.value = true;
  const isEditing = !!localProduct.value.idProduct;

  try {
    const formData = new FormData();
    formData.append('code', localProduct.value.code ?? '');
    formData.append('description', localProduct.value.description ?? '');
    formData.append('material', localProduct.value.material ?? '');
    formData.append('color', localProduct.value.color ?? '');
    formData.append('unitMeasure', localProduct.value.unitMeasure ?? '');
    formData.append('idBrand', String(localProduct.value.idBrand));
    formData.append('idCategory', String(localProduct.value.idCategory));
    formData.append('removeImage', imageDeleted.value ? 'true' : 'false');

    if (selectedImage.value)
      formData.append('image', selectedImage.value);

    let result;
    if (isEditing && localProduct.value.idProduct !== null) {
      result = await productStore.editProduct(localProduct.value.idProduct, formData);
    } else {
      result = await productStore.registerProduct(formData);
    }

    if (result.isSuccess) {
      toast.success(isEditing ? 'Producto editado con éxito!' : 'Producto agregado con éxito!');
      selectedImage.value = null;
      imageDeleted.value = false;
      emit('saved');
      close();
    }
  } catch (error: any) {
    handleApiError(error, isEditing ? 'Error en editar el producto' : 'Error en agregar el producto');
  } finally {
    saving.value = false;
  }
};

const handleGenerateCode = async () => {
  generatingCode.value = true;
  try {
    const result = await productStore.generateProductCode();
    if (result.isSuccess && result.data) {
      localProduct.value.code = result.data;
      toast.success('Código generado exitosamente');
    } else {
      toast.warning(result.message || 'No se pudo generar el código');
    }
  } catch (error) {
    handleApiError(error, 'Error al generar el código');
  } finally {
    generatingCode.value = false;
  }
};

const removeCurrentImage = () => {
  localProduct.value.image = '';
  imageDeleted.value = true;
};

const handleImageChange = (event: Event) => {
  const target = event.target as HTMLInputElement;
  selectedImage.value = target.files?.length ? target.files[0] : null;
};
</script>