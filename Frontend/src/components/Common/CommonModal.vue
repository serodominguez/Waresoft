<template>
    <v-dialog v-model="isOpen" max-width="400px" persistent>
        <v-card>
            <v-card-title class="bg-surface-light pt-4">
                {{ actionTitle }}
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text>
                Estás a punto de {{ actionVerb }} el ítem: {{ item }}.
            </v-card-text>
            <v-card-actions class="d-flex justify-space-between">
                <div class="d-flex">
                    <v-btn color="green" dark class="mr-2" elevation="4" @click="handleAction" :loading="processing">
                        Aceptar
                    </v-btn>
                    <v-btn color="red" elevation="4" @click="close" :disabled="processing">
                        Cancelar
                    </v-btn>
                </div>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useToast } from 'vue-toastification';
import { handleApiError } from '@/helpers/errorHandler';
import { useStoreMapper } from '@/composables/useStoreMapper';

// Props del componente
interface Props {
    modelValue: boolean;
    itemId: number;
    item: string;
    action: 0 | 1 | 2 | 3;
    moduleName: string;
    entityName: string;
    name: string;
    gender?: 'male' | 'female';
}

const props = withDefaults(defineProps<Props>(), {
    gender: 'male'
});

// Emits del componente
const emit = defineEmits<{
    'update:modelValue': [value: boolean];
    'action-completed': [];
}>();

// Inicialización de servicios
const toast = useToast();
const { getStoreAction } = useStoreMapper();

// Estado reactivo
const processing = ref(false);

// Computed bidireccional para v-model
const isOpen = computed({
    get: () => props.modelValue,
    set: (value: boolean) => emit('update:modelValue', value)
});

// Computed para textos dinámicos según la acción
const actionTitle = computed(() => {
    const titles = {
        0: 'Eliminar Item?',
        1: 'Activar Item?',
        2: 'Inactivar Item?',
        3: 'Cancelar Item?'
    };
    return titles[props.action];
});

const actionVerb = computed(() => {
    const verbs = {
        0: 'eliminar',
        1: 'activar',
        2: 'inactivar',
        3: 'cancelar'
    };
    return verbs[props.action];
});

/**
 * Cierra el modal
 */
const close = () => {
    isOpen.value = false;
};

/**
 * Maneja la ejecución de la acción seleccionada
 */
const handleAction = async () => {
    processing.value = true;

    const genderSuffix = props.gender === 'female' ? 'a' : 'o';

    const actionMap = {
        0: {
            actionType: 'eliminar' as const,
            successMsg: `${props.name} eliminad${genderSuffix} con éxito!`,
            errorMsg: `Error al eliminar ${props.name.toLowerCase()}`
        },
        1: {
            actionType: 'activar' as const,
            successMsg: `${props.name} activad${genderSuffix} con éxito!`,
            errorMsg: `Error al activar ${props.name.toLowerCase()}`
        },
        2: {
            actionType: 'inactivar' as const,
            successMsg: `${props.name} inactivad${genderSuffix} con éxito!`,
            errorMsg: `Error al inactivar ${props.name.toLowerCase()}`
        },
        3: { 
            actionType: 'cancelar' as const,
            successMsg: `${props.name} cancelad${genderSuffix} con éxito!`,
            errorMsg: `Error al cancelar ${props.name.toLowerCase()}`
        }
    };

    const currentAction = actionMap[props.action];

    try {
        // Obtener la acción del store correspondiente
        const storeAction = getStoreAction(
            props.moduleName,
            currentAction.actionType,
            props.entityName
        );

        const result = await storeAction(props.itemId);

        if (result.isSuccess) {
            toast.success(currentAction.successMsg);
            emit('action-completed');
            close();
        }
    } catch (error: any) {
        handleApiError(error, currentAction.errorMsg);
    } finally {
        processing.value = false;
    }
};
</script>