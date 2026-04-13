<template>
  <v-dialog v-model="dialogModel" :max-width="smAndDown ? '95vw' : '600px'" persistent>
    <v-card style="overflow: hidden;">
      <v-toolbar color="indigo" density="compact">
        <v-toolbar-title class="text-white text-body-2">
          <v-icon icon="mdi-image" class="mr-2" />
          {{ productCode || 'Imagen del producto' }}
        </v-toolbar-title>
        <v-spacer />
        <!-- Controles de zoom -->
        <v-btn icon color="white" :disabled="scale <= MIN_SCALE" @click="zoomOut">
          <v-icon icon="mdi-minus" />
        </v-btn>
        <span class="text-white text-body-2 mx-1">{{ Math.round(scale * 100) }}%</span>
        <v-btn icon color="white" :disabled="scale >= MAX_SCALE" @click="zoomIn">
          <v-icon icon="mdi-plus" />
        </v-btn>
        <v-btn icon color="white" @click="resetZoom">
          <v-icon icon="mdi-refresh" />
        </v-btn>
        <v-btn icon color="white" @click="dialogModel = false">
          <v-icon icon="mdi-close" />
        </v-btn>
      </v-toolbar>
      <v-card-text class="pa-4 d-flex align-center justify-center" style="height: 40vh; overflow: hidden; cursor: grab;"
        ref="containerRef" @wheel.prevent="onWheel" @mousedown="onMouseDown" @mousemove="onMouseMove"
        @mouseup="onMouseUp" @mouseleave="onMouseUp">
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
        <v-img v-else :src="imageSrc" contain :style="{
          height: '100%',
          width: '100%',
          transform: `scale(${scale}) translate(${translateX}px, ${translateY}px)`,
          transformOrigin: 'center center',
          transition: isDragging ? 'none' : 'transform 0.15s ease',
          cursor: isDragging ? 'grabbing' : 'grab',
          userSelect: 'none',
        }" @error="imageError = true">
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

const MIN_SCALE = 1;
const MAX_SCALE = 4;
const ZOOM_STEP = 0.25;

const imageError = ref(false);
const scale = ref(1);
const translateX = ref(0);
const translateY = ref(0);
const isDragging = ref(false);
const lastMouseX = ref(0);
const lastMouseY = ref(0);

const resetZoom = () => {
  scale.value = 1;
  translateX.value = 0;
  translateY.value = 0;
};

const zoomIn = () => {
  scale.value = Math.min(MAX_SCALE, scale.value + ZOOM_STEP);
};

const zoomOut = () => {
  scale.value = Math.max(MIN_SCALE, scale.value - ZOOM_STEP);
  if (scale.value === MIN_SCALE) {
    translateX.value = 0;
    translateY.value = 0;
  }
};

const onWheel = (e: WheelEvent) => {
  if (e.deltaY < 0) zoomIn();
  else zoomOut();
};

const onMouseDown = (e: MouseEvent) => {
  if (scale.value <= MIN_SCALE) return;
  isDragging.value = true;
  lastMouseX.value = e.clientX;
  lastMouseY.value = e.clientY;
};

const onMouseMove = (e: MouseEvent) => {
  if (!isDragging.value) return;
  translateX.value += (e.clientX - lastMouseX.value) / scale.value;
  translateY.value += (e.clientY - lastMouseY.value) / scale.value;
  lastMouseX.value = e.clientX;
  lastMouseY.value = e.clientY;
};

const onMouseUp = () => {
  isDragging.value = false;
};

watch(() => props.imageSrc, () => {
  imageError.value = false;
  resetZoom();
});

watch(() => props.modelValue, (val) => {
  if (!val) resetZoom();
});

const dialogModel = computed({
  get: () => props.modelValue,
  set: (value: boolean) => emit('update:modelValue', value),
});
</script>