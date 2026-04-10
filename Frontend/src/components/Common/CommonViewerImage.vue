<template>
  <v-dialog v-model="dialogModel" :max-width="smAndDown ? '95vw' : '600px'" persistent>
    <v-card style="overflow: hidden;">
      <v-toolbar color="indigo" density="compact">
        <v-toolbar-title class="text-white text-body-2">
          <v-icon icon="mdi-image" class="mr-2" />
          {{ productCode || 'Imagen del producto' }}
        </v-toolbar-title>
        <v-spacer />
        <v-btn icon color="white" @click="dialogModel = false">
          <v-icon icon="mdi-close" />
        </v-btn>
      </v-toolbar>

      <v-card-text class="pa-4 d-flex align-center justify-center" style="height: 40vh;">
        <!-- Error -->
        <div v-if="imageError" class="d-flex flex-column align-center justify-center" style="height: 100%;">
          <v-icon icon="mdi-image-broken-variant" color="red" size="52" class="mb-3" />
          <span class="text-body-1 text-red">No se pudo cargar la imagen</span>
        </div>
        <!-- Sin imagen -->
        <div v-else-if="!imageSrc" class="d-flex flex-column align-center justify-center" style="height: 100%;">
          <v-icon icon="mdi-image-off" color="grey" size="52" class="mb-3" />
          <span class="text-body-1 text-grey">Sin imagen disponible</span>
        </div>
        <!-- Imagen lista -->
        <v-img
          v-else
          :src="imageSrc"
          contain
          style="height: 100%; width: 100%;"
          @error="imageError = true"
        >
          <template v-slot:placeholder>
            <div class="d-flex align-center justify-center fill-height">
              <v-progress-circular indeterminate color="indigo" size="52" />
            </div>
          </template>
        </v-img>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useDisplay } from 'vuetify';

interface Props {
  modelValue: boolean;
  imageSrc?: string | null;
  productCode?: string | null;
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: false,
  imageSrc: null,
  productCode: null,
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
}>();

const { smAndDown } = useDisplay();

const imageError = ref(false);

watch(() => props.imageSrc, () => {
  imageError.value = false;
});

const dialogModel = computed({
  get: () => props.modelValue,
  set: (value: boolean) => emit('update:modelValue', value),
});
</script>