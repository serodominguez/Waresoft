<template>
  <v-container fluid class="dashboard pa-4">
    <div class="mb-4">
      <h1 class="dashboard-title">Dashboard</h1>
      <span class="dashboard-subtitle">Resumen general del sistema</span>
    </div>
    <v-row class="mb-4">

      <!-- Tarjeta 1: Traspasos por Sucursal (Barras) -->
      <v-col cols="12" md="6">
        <v-card class="chart-card" elevation="2" rounded="lg">
          <v-card-item>
            <template v-slot:prepend>
              <v-avatar color="indigo" size="40" class="mr-2">
                <v-icon icon="mdi-store" color="white" size="20"></v-icon>
              </v-avatar>
            </template>
            <v-card-title class="card-title">Traspasos por Sucursal</v-card-title>
            <v-card-subtitle>Últimos 6 meses</v-card-subtitle>
          </v-card-item>
          <v-card-text>
            <Bar v-if="!dashboard.loading && dashboard.transfersByStore.length" :data="barChartData"
              :options="barChartOptions" style="height: 160px;" />
            <div v-else class="d-flex align-center justify-center" style="height: 160px;">
              <v-progress-circular indeterminate color="indigo" />
            </div>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Tarjeta 2: Traspasos Recientes (Líneas) -->
      <v-col cols="12" md="6">
        <v-card class="chart-card" elevation="2" rounded="lg">
          <v-card-item>
            <template v-slot:prepend>
              <v-avatar color="teal" size="40" class="mr-2">
                <v-icon icon="mdi-trending-up" color="white" size="20"></v-icon>
              </v-avatar>
            </template>
            <v-card-title class="card-title">Traspasos Recientes</v-card-title>
            <v-card-subtitle>Últimos 6 meses</v-card-subtitle>
          </v-card-item>
          <v-card-text>
            <Line v-if="!dashboard.loading && dashboard.transferStatus.length" :data="lineChartData"
              :options="lineChartOptions" style="height: 160px;" />
            <div v-else class="d-flex align-center justify-center" style="height: 160px;">
              <v-progress-circular indeterminate color="teal" />
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
    <v-row class="mb-4">

      <!-- Tarjeta 3: Estado de Productos (Dona) -->
      <v-col cols="12" md="6">
        <v-card class="chart-card" elevation="2" rounded="lg">
          <v-card-item>
            <template v-slot:prepend>
              <v-avatar color="teal" size="40" class="mr-2">
                <v-icon icon="mdi-chart-donut" color="white" size="20"></v-icon>
              </v-avatar>
            </template>
            <v-card-title class="card-title">Estado de Productos</v-card-title>
            <v-card-subtitle>Disponibilidad actual</v-card-subtitle>
          </v-card-item>
          <v-card-text>
            <Doughnut v-if="!dashboard.loading && dashboard.productReplenishment" :data="doughnutChartData"
              :options="doughnutChartOptions" style="height: 160px;" />
            <div v-else class="d-flex align-center justify-center" style="height: 160px;">
              <v-progress-circular indeterminate color="teal" />
            </div>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Tarjeta 4: Movimientos (Barras apiladas) -->
      <v-col cols="12" md="6">
        <v-card class="chart-card" elevation="2" rounded="lg">
          <v-card-item>
            <template v-slot:prepend>
              <v-avatar color="deep-orange" size="40" class="mr-2">
                <v-icon icon="mdi-swap-horizontal" color="white" size="20"></v-icon>
              </v-avatar>
            </template>
            <v-card-title class="card-title">Movimientos</v-card-title>
            <v-card-subtitle>Entradas vs Salidas</v-card-subtitle>
          </v-card-item>
          <v-card-text>
            <Bar :data="stackedBarChartData" :options="stackedBarChartOptions" style="height: 160px;" />
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
    <v-row>

      <!-- Stat 1: Productos -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="stat-card stat-card--blue" elevation="2" rounded="lg">
          <v-card-text class="d-flex align-center justify-space-between pa-5">
            <div>
              <div class="stat-value">
                {{ dashboard.loading ? '...' : (dashboard.productStats?.totalActive ?? 0) }}
              </div>
              <div class="stat-label">Productos</div>
              <div v-if="!dashboard.loading && dashboard.productStats" class="stat-change positive">
                <v-icon icon="mdi-arrow-up" size="14" />
                +{{ dashboard.productStats.newThisMonth }} nuevos este mes
              </div>
            </div>
            <v-avatar color="indigo" size="56" class="stat-icon">
              <v-icon icon="mdi-package-variant-closed" color="white" size="28"></v-icon>
            </v-avatar>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Stat: Productos bajo mínimo -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="stat-card stat-card--red" elevation="2" rounded="lg">
          <v-card-text class="d-flex align-center justify-space-between pa-5">
            <div>
              <div class="stat-value">
                {{ dashboard.loading ? '...' : (dashboard.inventoryStats?.belowMinimum ?? 0) }}
              </div>
              <div class="stat-label">Existencias Mínimo</div>
              <div v-if="!dashboard.loading && dashboard.inventoryStats"
                :class="['stat-change', dashboard.inventoryStats.isPositive ? 'positive' : 'negative']">
                <v-icon :icon="dashboard.inventoryStats.isPositive ? 'mdi-arrow-up' : 'mdi-arrow-down'" size="14" />
                {{ dashboard.inventoryStats.isPositive ? '+' : '-' }}{{ dashboard.inventoryStats.differenceVsLastMonth
                }}
                este mes
              </div>
            </div>
            <v-avatar color="red-darken-1" size="56" class="stat-icon">
              <v-icon icon="mdi-alert-circle-outline" color="white" size="28"></v-icon>
            </v-avatar>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Stat 3: Salidas -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="stat-card stat-card--green" elevation="2" rounded="lg">
          <v-card-text class="d-flex align-center justify-space-between pa-5">
            <div>
              <div class="stat-value">
                {{ dashboard.loading ? '...' : (dashboard.goodsIssueStats?.totalIssues ?? 0) }}
              </div>
              <div class="stat-label">Salidas</div>
              <div v-if="!dashboard.loading && dashboard.goodsIssueStats"
                :class="['stat-change', dashboard.goodsIssueStats.isPositive ? 'positive' : 'negative']">
                <v-icon :icon="dashboard.goodsIssueStats.isPositive ? 'mdi-arrow-up' : 'mdi-arrow-down'" size="14" />
                {{ dashboard.goodsIssueStats.isPositive ? '+' : '-' }}{{ dashboard.goodsIssueStats.differenceVsLast7Days
                }}
                vs últ. 7 días
              </div>
            </div>
            <v-avatar color="green-darken-2" size="56" class="stat-icon">
              <v-icon icon="mdi-arrow-collapse-up" color="white" size="28"></v-icon>
            </v-avatar>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Stat 4: Traspasos Pendientes -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="stat-card stat-card--orange" elevation="2" rounded="lg">
          <v-card-text class="d-flex align-center justify-space-between pa-5">
            <div>
              <div class="stat-value">
                {{ dashboard.loading ? '...' : (dashboard.transferPending?.totalPending ?? 0) }}
              </div>
              <div class="stat-label">Traspasos Pendientes</div>
              <div v-if="!dashboard.loading && dashboard.transferPending"
                :class="['stat-change', dashboard.transferPending.isPendingPositive ? 'positive' : 'negative']">
                <v-icon :icon="dashboard.transferPending.isPendingPositive ? 'mdi-arrow-up' : 'mdi-arrow-down'"
                  size="14" />
                {{ dashboard.transferPending.isPendingPositive ? '+' : '-' }}{{
                  dashboard.transferPending.differenceVsYesterday
                }} vs ayer
              </div>
            </div>
            <v-avatar color="deep-orange" size="56" class="stat-icon">
              <v-icon icon="mdi-clipboard-list" color="white" size="28"></v-icon>
            </v-avatar>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted, computed } from 'vue';
import { useDashboardStore } from '@/stores/dashboardStore';
import { Bar, Line, Doughnut } from 'vue-chartjs';
import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, LineElement, PointElement, CategoryScale, LinearScale, Filler, ArcElement } from 'chart.js';

ChartJS.register(
  Title, Tooltip, Legend,
  BarElement, LineElement, PointElement,
  CategoryScale, LinearScale, Filler,
  ArcElement
);

const dashboard = useDashboardStore();
onMounted(() => dashboard.fetchAll());

const stackedBarChartData = computed(() => ({
  labels: dashboard.movementsStats.map(m => m.month),
  datasets: [
    {
      label: 'Entradas',
      data: dashboard.movementsStats.map(m => m.receipts),
      backgroundColor: 'rgba(38, 166, 154, 0.8)',
      borderRadius: 4,
    },
    {
      label: 'Salidas',
      data: dashboard.movementsStats.map(m => m.issues),
      backgroundColor: 'rgba(239, 108, 0, 0.8)',
      borderRadius: 4,
    },
  ],
}));

const stackedBarChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: true,
      position: 'bottom' as const,
      labels: { boxWidth: 12, font: { size: 11 } },
    },
    tooltip: { mode: 'index' as const, intersect: false },
  },
  scales: {
    x: { stacked: true, grid: { display: false } },
    y: {
      stacked: true,
      grid: { color: 'rgba(0,0,0,0.05)' },
      ticks: { maxTicksLimit: 5, stepSize: 1, precision: 0 },
    },
  },
};

const doughnutChartData = computed(() => ({
  labels: ['Disponible', 'No Disponible', 'Descontinuado'],
  datasets: [
    {
      data: [
        dashboard.productReplenishment?.available    ?? 0,
        dashboard.productReplenishment?.notAvailable ?? 0,
        dashboard.productReplenishment?.discontinued ?? 0,
      ],
      backgroundColor: [
        'rgba(56, 161, 105, 0.8)',  // verde
        'rgba(237, 137, 54, 0.8)',  // naranja
        'rgba(229, 62, 62, 0.8)',   // rojo
      ],
      borderWidth: 1,
    },
  ],
}));

const doughnutChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: true,
      position: 'bottom' as const,
      labels: { boxWidth: 12, font: { size: 11 } },
    },
    tooltip: { mode: 'index' as const, intersect: false },
  },
};

const barChartData = computed(() => ({
  labels: dashboard.transfersByStore.map(s => s.storeName),
  datasets: [
    {
      label: 'Traspasos Recibidos',
      data: dashboard.transfersByStore.map(s => s.totalTransfers),
      backgroundColor: 'rgba(92, 107, 192, 0.8)',
      borderColor: 'rgba(92, 107, 192, 1)',
      borderWidth: 1,
      borderRadius: 6,
    },
  ],
}));

const barChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: { mode: 'index' as const, intersect: false },
  },
  scales: {
    x: { grid: { display: false } },
    y: {
      grid: { color: 'rgba(0,0,0,0.05)' },
      ticks: { maxTicksLimit: 5, stepSize: 1, precision: 0 },
    },
  },
};

const lineChartData = computed(() => ({
  labels: dashboard.transferStatus.map(t => t.month),
  datasets: [
  {
    label: 'Sent',
    data: dashboard.transferStatus.map(t => t.sent),
    borderColor: 'rgba(92, 107, 192, 1)',
    backgroundColor: 'rgba(92, 107, 192, 0.1)',
    borderWidth: 2,
    pointBackgroundColor: 'rgba(92, 107, 192, 1)',
    pointRadius: 4,
    tension: 0.4,
    fill: true,
  },
  {
    label: 'Pending',
    data: dashboard.transferStatus.map(t => t.pending),
    borderColor: 'rgba(237, 137, 54, 1)',
    backgroundColor: 'rgba(237, 137, 54, 0.1)',
    borderWidth: 2,
    pointBackgroundColor: 'rgba(237, 137, 54, 1)',
    pointRadius: 5,  // punto más grande para que sea visible
    pointHoverRadius: 7,
    tension: 0.4,
    fill: false,  // sin relleno para que no se oculte
  },
  {
    label: 'Received',
    data: dashboard.transferStatus.map(t => t.received),
    borderColor: 'rgba(56, 161, 105, 1)',
    backgroundColor: 'rgba(56, 161, 105, 0.1)',
    borderWidth: 2,
    pointBackgroundColor: 'rgba(56, 161, 105, 1)',
    pointRadius: 5,
    pointHoverRadius: 7,
    tension: 0.4,
    fill: false,
  },
],
}));

const lineChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: true,
      position: 'bottom' as const,
      labels: { boxWidth: 12, font: { size: 11 } },
    },
    tooltip: { mode: 'index' as const, intersect: false },
  },
  scales: {
    x: { grid: { display: false } },
    y: {
      grid: { color: 'rgba(0,0,0,0.05)' },
      ticks: { maxTicksLimit: 5, stepSize: 1, precision: 0 },
    },
  },
};
</script>

<style scoped>
.dashboard {
  background-color: #f5f6fa;
  min-height: 100%;
}

.dashboard-title {
  font-size: 1.6rem;
  font-weight: 700;
  color: rgb(26, 32, 44);
  line-height: 0.4;
}

.dashboard-subtitle {
  font-size: 0.9rem;
  color: #718096;
}

.chart-card {
  background: #ffffff;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.chart-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1) !important;
}

.card-title {
  font-size: 0.95rem !important;
  font-weight: 600;
  color: rgb(26, 32, 44);
}

.stat-card {
  background: #ffffff;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
  border-left: 4px solid transparent;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1) !important;
}

.stat-card--blue   { border-left-color: #5c6bc0; }
.stat-card--red    { border-left-color: #e53e3e; }
.stat-card--green  { border-left-color: #388e3c; }
.stat-card--orange { border-left-color: #e64a19; }

.stat-value {
  font-size: 1.7rem;
  font-weight: 700;
  color: rgb(26, 32, 44);
  line-height: 1.1;
}

.stat-label {
  font-size: 0.85rem;
  color: #718096;
  margin-top: 2px;
  margin-bottom: 4px;
}

.stat-change {
  font-size: 0.78rem;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 2px;
}

.stat-change.positive { color: #38a169; }
.stat-change.negative { color: #e53e3e; }
</style>