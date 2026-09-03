<template>
    <div>
        <v-card elevation="2">
            <v-data-table-server :headers="headers" :items="periods" :items-per-page-text="pages"
                :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalPeriods"
                :loading="loading" loading-text="Cargando... Espere por favor"
                @update:items-per-page="$emit('update-items-per-page', $event)"
                @update:page="$emit('change-page', $event)">
                <template v-slot:item="{ item }">
                    <tr>
                        <td>{{ (item as InventoryPeriod).periodName }}</td>
                        <td>{{ (item as InventoryPeriod).startDate }}</td>
                        <td>{{ (item as InventoryPeriod).endDate }}</td>
                        <td>{{ (item as InventoryPeriod).openedDate }}</td>
                        <td>{{ (item as InventoryPeriod).closedDate ?? '—' }}</td>
                        <td>
                            <v-chip :color="statusColor((item as InventoryPeriod).statusPeriod)" variant="tonal"
                                size="small" rounded="lg">
                                <v-icon :icon="statusIcon((item as InventoryPeriod).statusPeriod)" start
                                    size="12"></v-icon>
                                {{ statusLabel((item as InventoryPeriod).statusPeriod) }}
                            </v-chip>
                        </td>
                        <td class="text-center">
                            <v-tooltip v-bind="tooltipProps" text="Imprimir Apertura" location="bottom">
                                <template v-slot:activator="{ props }">
                                    <v-btn v-bind="props" icon variant="text" color="grey-darken-1" size="small"
                                        @click="$emit('print-opening-pdf', item)"
                                        :loading="printingPdfId === (item as InventoryPeriod).idPeriod"
                                        :disabled="printingPdfId === (item as InventoryPeriod).idPeriod">
                                        <v-icon icon="mdi-file-lock-open-outline" size="24"></v-icon>
                                    </v-btn>
                                </template>
                            </v-tooltip>
                            <v-tooltip v-bind="tooltipProps" text="Imprimir Cierre" location="bottom"
                                v-if="(item as InventoryPeriod).statusPeriod === 'Cerrado'">
                                <template v-slot:activator="{ props }">
                                    <v-btn v-bind="props" icon variant="text" color="grey-darken-1" size="small"
                                        @click="$emit('print-closing-pdf', item)"
                                        :loading="printingPdfId === (item as InventoryPeriod).idPeriod"
                                        :disabled="printingPdfId === (item as InventoryPeriod).idPeriod">
                                        <v-icon icon="mdi-file-lock-outline" size="24"></v-icon>
                                    </v-btn>
                                </template>
                            </v-tooltip>
                            <v-tooltip v-bind="tooltipProps" text="Cerrar Período" location="bottom">
                                <template v-slot:activator="{ props }">
                                    <v-btn v-bind="props"
                                        v-if="canEdit && (item as InventoryPeriod).statusPeriod === 'Abierto'" icon
                                        variant="text" color="grey-darken-1" size="small"
                                        @click="$emit('open-close-modal', item)">
                                        <v-icon icon="mdi-lock" size="24"></v-icon>
                                    </v-btn>
                                </template>
                            </v-tooltip>
                        </td>
                    </tr>
                </template>
                <template v-slot:top>
                    <v-toolbar>
                        <v-toolbar-title>
                            <v-avatar color="indigo" size="36" class="mr-3">
                                <v-icon icon="mdi-calendar-clock" color="white" size="18"></v-icon>
                            </v-avatar>
                            Gestión de Períodos
                        </v-toolbar-title>
                        <v-spacer></v-spacer>
                        <v-tooltip v-bind="tooltipProps" text="Descargar PDF" location="bottom">
                            <template v-slot:activator="{ props }">
                                <v-btn v-bind="props" v-if="canDownload" icon variant="text" color="red" size="38"
                                    @click="handleDownloadPdf" :loading="downloadingPdf" :disabled="downloadingPdf"
                                    class="mr-2">
                                    <v-icon icon="mdi-file-pdf-box" size="26"></v-icon>
                                </v-btn>
                            </template>
                        </v-tooltip>
                        <v-tooltip v-bind="tooltipProps" text="Descargar Excel" location="bottom">
                            <template v-slot:activator="{ props }">
                                <v-btn v-bind="props" v-if="canDownload" icon variant="text" color="green" size="38"
                                    @click="handleDownloadExcel" :loading="downloadingExcel"
                                    :disabled="downloadingExcel" class="mr-2">
                                    <v-icon icon="mdi-file-excel-box" size="26"></v-icon>
                                </v-btn>
                            </template>
                        </v-tooltip>
                        <v-tooltip v-bind="tooltipProps" text="Abrir Período" location="bottom">
                            <template v-slot:activator="{ props }">
                                <v-btn v-bind="props" v-if="canCreate" icon variant="text" color="indigo" size="38"
                                    @click="$emit('open-form')" class="mr-2">
                                    <v-icon icon="mdi-plus-box" size="26"></v-icon>
                                </v-btn>
                            </template>
                        </v-tooltip>
                        <v-tooltip v-bind="tooltipProps" text="Filtros" location="bottom">
                            <template v-slot:activator="{ props }">
                                <v-btn v-bind="props" icon variant="text" size="38" @click="drawerModel = !drawerModel"
                                    class="mr-4">
                                    <v-icon icon="mdi-tune-variant" size="26"></v-icon>
                                </v-btn>
                            </template>
                        </v-tooltip>
                        <v-text-field density="compact" label="Búsqueda" variant="solo" hide-details single-line
                            v-model="search" class="mr-4" style="width: 100%; max-width: 300px;"
                            @keyup.enter="handleSearch()">
                            <template v-slot:append-inner>
                                <v-tooltip v-bind="tooltipProps" text="Buscar" location="bottom">
                                    <template v-slot:activator="{ props }">
                                        <v-icon v-bind="props" icon="mdi-magnify" @click="handleSearch()"
                                            style="cursor: pointer;"></v-icon>
                                    </template>
                                </v-tooltip>
                            </template>
                        </v-text-field>
                    </v-toolbar>
                </template>
                <template v-slot:no-data>
                    <span class="text-grey">No se encontraron períodos</span>
                </template>
            </v-data-table-server>
        </v-card>
        <CommonFiltersMovements v-model="drawerModel" :filters="filterOptions"
            v-model:selected-filter="selectedFilterModel" :status-options="PeriodStatusOptions"
            v-model:state="stateModel" v-model:start-date="startDateModel" v-model:end-date="endDateModel"
            @apply-filters="handleSearch" @clear-filters="handleClearFilters" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { InventoryPeriod } from '@/interfaces/inventoryPeriodInterface';
import CommonFiltersMovements from '@/components/Common/CommonFiltersMovements.vue';
import { useResponsiveTooltip } from '@/composables/useResponsiveTooltip';

const PeriodStatusOptions = ['Abierto', 'Cerrado', 'Cancelado'];

const props = withDefaults(defineProps<{
    canCreate: boolean;
    canEdit: boolean;
    canDownload?: boolean;
    drawer: boolean;
    selectedFilter: string;
    state: string;
    startDate: Date | null;
    endDate: Date | null;
    periods: InventoryPeriod[];
    totalPeriods: number;
    loading: boolean;
    itemsPerPage?: number;
    downloadingExcel?: boolean;
    downloadingPdf?: boolean;
    printingPdfId?: number | null;
}>(), {
    canDownload: false,
    drawer: false,
    selectedFilter: 'Período',
    state: 'Todos',
    startDate: null,
    endDate: null,
    itemsPerPage: 10,
    downloadingExcel: false,
    downloadingPdf: false,
    printingPdfId: null,
});

const emit = defineEmits<{
    'open-form': [];
    'open-close-modal': [period: InventoryPeriod];
    'update-items-per-page': [value: number];
    'change-page': [page: number];
    'search-periods': [params: {
        search: string | null;
        selectedFilter: string;
        state: string;
        startDate: Date | null;
        endDate: Date | null;
    }];
    'download-excel': [params: { search: string | null }];
    'download-pdf': [params: { search: string | null }];
    'update:drawer': [value: boolean];
    'update:selectedFilter': [value: string];
    'update:state': [value: string];
    'update:startDate': [value: Date | null];
    'update:endDate': [value: Date | null];
    'clear-filters': [];
    'print-opening-pdf': [period: InventoryPeriod];
    'print-closing-pdf':  [period: InventoryPeriod];
}>();

const { tooltipProps } = useResponsiveTooltip();

const pages = 'Períodos por Página';
const filterOptions = ['Período'];
const search = ref<string | null>(null);

const headers = computed(() => [
    { title: 'Período',        key: 'periodName',   sortable: false },
    { title: 'Fecha de apertura',   key: 'startDate',    sortable: false },
    { title: 'Fecha de cierre',      key: 'endDate',      sortable: false },
    { title: 'Fecha de creación', key: 'openedDate',   sortable: false },
    { title: 'Fecha de conclusión',   key: 'closedDate',   sortable: false },
    { title: 'Estado',         key: 'status',       sortable: false },
    { title: 'Acciones',       key: 'actions',      sortable: false, align: 'center' as const },
]);

const drawerModel = computed({
    get: () => props.drawer,
    set: (v: boolean) => emit('update:drawer', v),
});
const selectedFilterModel = computed({
    get: () => props.selectedFilter,
    set: (v: string) => emit('update:selectedFilter', v),
});
const stateModel = computed({
    get: () => props.state,
    set: (v: string) => emit('update:state', v),
});
const startDateModel = computed({
    get: () => props.startDate,
    set: (v: Date | null) => emit('update:startDate', v),
});
const endDateModel = computed({
    get: () => props.endDate,
    set: (v: Date | null) => emit('update:endDate', v),
});

const statusColor = (status: string): string => {
    if (status === 'Abierto')   return 'green';
    if (status === 'Cerrado')   return 'red';
    if (status === 'Cancelado') return 'grey';
    return 'grey';
};
const statusIcon = (status: string): string => {
    if (status === 'Abierto')   return 'mdi-lock-open';
    if (status === 'Cerrado')   return 'mdi-lock';
    if (status === 'Cancelado') return 'mdi-cancel';
    return 'mdi-circle-outline';
};
const statusLabel = (status: string): string => status; // el backend ya devuelve "Abierto" etc.

const handleSearch = () => {
    emit('search-periods', {
        search: search.value,
        selectedFilter: selectedFilterModel.value,
        state: stateModel.value,
        startDate: startDateModel.value,
        endDate: endDateModel.value,
    });
};
const handleClearFilters = () => {
    search.value = null;
    emit('clear-filters');
};
const handleDownloadExcel = () => emit('download-excel', { search: search.value });
const handleDownloadPdf   = () => emit('download-pdf',   { search: search.value });
</script>