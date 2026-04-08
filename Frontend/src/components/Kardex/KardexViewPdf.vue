<template>
    <v-dialog v-model="model" max-width="960" persistent>
        <v-card style="overflow: hidden;">
            <v-toolbar color="indigo" density="compact">
                <v-toolbar-title class="text-white text-body-2">
                    <v-icon icon="mdi-file-pdf-box" class="mr-2" />
                    {{ title }}
                </v-toolbar-title>
                <v-spacer />
                <v-btn icon color="white" @click="handleClose">
                    <v-icon icon="mdi-close" />
                </v-btn>
            </v-toolbar>
            <v-card-text class="pa-0" style="height: 78vh;">
                <!-- Error -->
                <div v-if="error" class="d-flex flex-column align-center justify-center" style="height: 100%;">
                    <v-icon icon="mdi-alert-circle-outline" color="red" size="52" class="mb-3" />
                    <span class="text-body-1 text-red">No se pudo cargar el documento PDF</span>
                </div>
                <!-- Cargando -->
                <div v-else-if="!url" class="d-flex align-center justify-center" style="height: 100%;">
                    <v-progress-circular indeterminate color="indigo" size="52" />
                </div>
                <!-- PDF listo -->
                <iframe v-else :src="url" width="100%" style="height: 100%; border: none;" title="Documento PDF" />
            </v-card-text>
        </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
interface Props {
  modelValue: boolean;
  title?: string;
  url?: string | null;
  error?: boolean;
}
 
withDefaults(defineProps<Props>(), {
  title: 'Documento PDF',
  url: null,
  error: false,
});
 
const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'close': [];
}>();
 
const model = defineModel<boolean>('modelValue');
 
const handleClose = () => {
  model.value = false;
  emit('close');
};
</script>