<template>
  <v-container fluid class="dashboard pa-4">

    <!-- Título -->
    <div class="mb-6">
      <h1 class="dashboard-title">Dashboard</h1>
      <span class="dashboard-subtitle">Resumen general del sistema</span>
    </div>

    <!-- Fila superior: Tarjetas con gráficas -->
    <v-row class="mb-4">

      <!-- Tarjeta 1: Ventas mensuales (Barras) -->
      <v-col cols="12" md="4">
        <v-card class="chart-card" elevation="2" rounded="lg">
          <v-card-item>
            <template v-slot:prepend>
              <v-avatar color="indigo" size="40" class="mr-2">
                <v-icon icon="mdi-chart-bar" color="white" size="20"></v-icon>
              </v-avatar>
            </template>
            <v-card-title class="card-title">Ventas Mensuales</v-card-title>
            <v-card-subtitle>Últimos 6 meses</v-card-subtitle>
          </v-card-item>
          <v-card-text>
            <Bar :data="barChartData" :options="barChartOptions" style="height: 180px;" />
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Tarjeta 2: Ingresos (Líneas) -->
      <v-col cols="12" md="4">
        <v-card class="chart-card" elevation="2" rounded="lg">
          <v-card-item>
            <template v-slot:prepend>
              <v-avatar color="teal" size="40" class="mr-2">
                <v-icon icon="mdi-trending-up" color="white" size="20"></v-icon>
              </v-avatar>
            </template>
            <v-card-title class="card-title">Ingresos</v-card-title>
            <v-card-subtitle>Tendencia anual</v-card-subtitle>
          </v-card-item>
          <v-card-text>
            <Line :data="lineChartData" :options="lineChartOptions" style="height: 180px;" />
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Tarjeta 3: Movimientos (Barras apiladas) -->
      <v-col cols="12" md="4">
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
            <Bar :data="stackedBarChartData" :options="stackedBarChartOptions" style="height: 180px;" />
          </v-card-text>
        </v-card>
      </v-col>

    </v-row>

    <!-- Fila inferior: Tarjetas de estadísticas -->
    <v-row>

      <!-- Stat 1: Clientes -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="stat-card stat-card--blue" elevation="2" rounded="lg">
          <v-card-text class="d-flex align-center justify-space-between pa-5">
            <div>
              <div class="stat-value">245</div>
              <div class="stat-label">Clientes</div>
              <div class="stat-change positive">
                <v-icon icon="mdi-arrow-up" size="14"></v-icon> +12% este mes
              </div>
            </div>
            <v-avatar color="indigo" size="56" class="stat-icon">
              <v-icon icon="mdi-account-group" color="white" size="28"></v-icon>
            </v-avatar>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Stat 2: Productos -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="stat-card stat-card--teal" elevation="2" rounded="lg">
          <v-card-text class="d-flex align-center justify-space-between pa-5">
            <div>
              <div class="stat-value">1,521</div>
              <div class="stat-label">Productos</div>
              <div class="stat-change positive">
                <v-icon icon="mdi-arrow-up" size="14"></v-icon> +38 nuevos
              </div>
            </div>
            <v-avatar color="teal" size="56" class="stat-icon">
              <v-icon icon="mdi-package-variant-closed" color="white" size="28"></v-icon>
            </v-avatar>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Stat 3: Ingresos -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="stat-card stat-card--green" elevation="2" rounded="lg">
          <v-card-text class="d-flex align-center justify-space-between pa-5">
            <div>
              <div class="stat-value">$34,245</div>
              <div class="stat-label">Ingresos</div>
              <div class="stat-change positive">
                <v-icon icon="mdi-arrow-up" size="14"></v-icon> +8.5% vs mes ant.
              </div>
            </div>
            <v-avatar color="green-darken-2" size="56" class="stat-icon">
              <v-icon icon="mdi-cash-multiple" color="white" size="28"></v-icon>
            </v-avatar>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Stat 4: Pedidos -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="stat-card stat-card--orange" elevation="2" rounded="lg">
          <v-card-text class="d-flex align-center justify-space-between pa-5">
            <div>
              <div class="stat-value">184</div>
              <div class="stat-label">Pedidos</div>
              <div class="stat-change negative">
                <v-icon icon="mdi-arrow-down" size="14"></v-icon> -3 vs ayer
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
import { Bar, Line } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  LineElement,
  PointElement,
  CategoryScale,
  LinearScale,
  Filler,
} from 'chart.js';

ChartJS.register(
  Title, Tooltip, Legend,
  BarElement, LineElement, PointElement,
  CategoryScale, LinearScale, Filler
);

const meses = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun'];

// ── Gráfica de barras: Ventas mensuales ──────────────────────────────────
const barChartData = {
  labels: meses,
  datasets: [
    {
      label: 'Ventas',
      data: [4200, 3800, 5100, 4700, 6200, 5800],
      backgroundColor: 'rgba(92, 107, 192, 0.8)',
      borderColor: 'rgba(92, 107, 192, 1)',
      borderWidth: 1,
      borderRadius: 6,
    },
  ],
};

const barChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: { mode: 'index' as const, intersect: false },
  },
  scales: {
    x: { grid: { display: false } },
    y: { grid: { color: 'rgba(0,0,0,0.05)' }, ticks: { maxTicksLimit: 5 } },
  },
};

// ── Gráfica de líneas: Ingresos ──────────────────────────────────────────
const lineChartData = {
  labels: meses,
  datasets: [
    {
      label: 'Ingresos ($)',
      data: [18000, 21000, 19500, 24000, 28000, 34245],
      borderColor: 'rgba(0, 150, 136, 1)',
      backgroundColor: 'rgba(0, 150, 136, 0.1)',
      borderWidth: 2,
      pointBackgroundColor: 'rgba(0, 150, 136, 1)',
      pointRadius: 4,
      tension: 0.4,
      fill: true,
    },
  ],
};

const lineChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: { mode: 'index' as const, intersect: false },
  },
  scales: {
    x: { grid: { display: false } },
    y: { grid: { color: 'rgba(0,0,0,0.05)' }, ticks: { maxTicksLimit: 5 } },
  },
};

// ── Gráfica de barras apiladas: Movimientos ──────────────────────────────
const stackedBarChartData = {
  labels: meses,
  datasets: [
    {
      label: 'Entradas',
      data: [320, 410, 380, 450, 500, 470],
      backgroundColor: 'rgba(38, 166, 154, 0.8)',
      borderRadius: 4,
    },
    {
      label: 'Salidas',
      data: [280, 350, 300, 390, 420, 410],
      backgroundColor: 'rgba(239, 108, 0, 0.8)',
      borderRadius: 4,
    },
  ],
};

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
    y: { stacked: true, grid: { color: 'rgba(0,0,0,0.05)' }, ticks: { maxTicksLimit: 5 } },
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
  line-height: 1.2;
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
.stat-card--teal   { border-left-color: #009688; }
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