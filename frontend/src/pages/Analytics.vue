<template>
  <div class="analytics-page">
    <div class="container">
      <h1>📊 Análitica</h1>

      <div class="date-range">
        <input v-model="startDate" type="date" class="input-date" />
        <span>a</span>
        <input v-model="endDate" type="date" class="input-date" />
        <button @click="loadAnalytics" class="btn btn-primary">Cargar</button>
      </div>

      <div class="metrics-grid">
        <div class="metric-card">
          <div class="metric-title">Órdenes Totales</div>
          <div class="metric-value">{{ metrics.totalOrders }}</div>
          <div class="metric-change" :class="metrics.ordersChange > 0 ? 'positive' : 'negative'">
            {{ metrics.ordersChange > 0 ? '↑' : '↓' }} {{ Math.abs(metrics.ordersChange) }}%
          </div>
        </div>

        <div class="metric-card">
          <div class="metric-title">Ingresos Totales</div>
          <div class="metric-value">${{ metrics.totalRevenue.toFixed(2) }}</div>
          <div class="metric-change" :class="metrics.revenueChange > 0 ? 'positive' : 'negative'">
            {{ metrics.revenueChange > 0 ? '↑' : '↓' }} {{ Math.abs(metrics.revenueChange) }}%
          </div>
        </div>

        <div class="metric-card">
          <div class="metric-title">Tasa de Éxito</div>
          <div class="metric-value">{{ metrics.successRate }}%</div>
          <div class="metric-subtitle">{{ metrics.completedOrders }} de {{ metrics.totalOrders }} completadas</div>
        </div>

        <div class="metric-card">
          <div class="metric-title">AOV (Valor Promedio)</div>
          <div class="metric-value">${{ metrics.averageOrderValue.toFixed(2) }}</div>
          <div class="metric-subtitle">por orden</div>
        </div>
      </div>

      <div class="charts-grid">
        <div class="chart-card">
          <h3>Órdenes por Día</h3>
          <div class="chart-placeholder">
            📈 Gráfico de línea mostrando órdenes diarias
          </div>
        </div>

        <div class="chart-card">
          <h3>Ingresos por Día</h3>
          <div class="chart-placeholder">
            💰 Gráfico de barras mostrando ingresos diarios
          </div>
        </div>

        <div class="chart-card">
          <h3>Tipos de Mensajes</h3>
          <div class="chart-placeholder">
            🥧 Gráfico de pie mostrando distribución
          </div>
        </div>

        <div class="chart-card">
          <h3>Mensajeros Top 5</h3>
          <div class="chart-placeholder">
            ⭐ Ranking de mensajeros por órdenes completadas
          </div>
        </div>
      </div>

      <div class="export-section">
        <h3>Exportar Reportes</h3>
        <div class="export-buttons">
          <button @click="exportCSV" class="btn btn-secondary">📥 Descargar CSV</button>
          <button @click="exportPDF" class="btn btn-secondary">📄 Descargar PDF</button>
        </div>
      </div>

      <div class="top-performers">
        <h3>Top 5 Mensajeros</h3>
        <table class="performers-table">
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Órdenes</th>
              <th>Ingresos</th>
              <th>Rating</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="performer in topPerformers" :key="performer.id">
              <td>{{ performer.name }}</td>
              <td>{{ performer.orders }}</td>
              <td>${{ performer.earnings.toFixed(2) }}</td>
              <td>⭐ {{ performer.rating.toFixed(1) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const startDate = ref('')
const endDate = ref('')

const metrics = ref({
  totalOrders: 0,
  totalRevenue: 0,
  successRate: 0,
  completedOrders: 0,
  averageOrderValue: 0,
  ordersChange: 0,
  revenueChange: 0
})

const topPerformers = ref([])

onMounted(() => {
  // Set default dates (last 30 days)
  const end = new Date()
  const start = new Date(end.getTime() - 30 * 24 * 60 * 60 * 1000)
  
  startDate.value = start.toISOString().split('T')[0]
  endDate.value = end.toISOString().split('T')[0]
  
  loadAnalytics()
})

const loadAnalytics = async () => {
  try {
    const response = await fetch(`/api/analytics?startDate=${startDate.value}&endDate=${endDate.value}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    if (response.ok) {
      const data = await response.json()
      metrics.value = data.metrics
      topPerformers.value = data.topPerformers
    }
  } catch (error) {
    console.error('Error loading analytics:', error)
  }
}

const exportCSV = () => {
  // Generate CSV with analytics data
  const csv = [
    ['Date Range', `${startDate.value} to ${endDate.value}`],
    ['Total Orders', metrics.value.totalOrders],
    ['Total Revenue', metrics.value.totalRevenue],
    ['Success Rate', `${metrics.value.successRate}%`],
    ['Average Order Value', metrics.value.averageOrderValue],
    ['', ''],
    ['Top Performers'],
    ['Name', 'Orders', 'Earnings', 'Rating']
  ]
  
  topPerformers.value.forEach(p => {
    csv.push([p.name, p.orders, p.earnings, p.rating])
  })
  
  const csvContent = csv.map(row => row.join(',')).join('\n')
  const blob = new Blob([csvContent], { type: 'text/csv' })
  const url = window.URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `analytics_${startDate.value}_${endDate.value}.csv`
  a.click()
}

const exportPDF = () => {
  alert('PDF export functionality to be implemented with PDF library')
  // Would use a library like jsPDF or similar
}
</script>

<style scoped>
/* Specific Analytics page styles only */
.analytics-page {
  background: #f9fafb;
}

.date-range {
  display: flex;
  gap: 1rem;
  align-items: center;
  margin-bottom: 2rem;
}

.input-date {
  padding: 0.75rem 1rem;
  border: 1px solid #e2e8f0;
  border-radius: 4px;
  font-size: 0.95rem;
}

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.metric-card {
  border-left: 4px solid var(--primary-color);
}

.metric-change.positive {
  color: #10b981;
}

.metric-change.negative {
  color: #ef4444;
}

.charts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.chart-placeholder {
  height: 250px;
  background: #f9fafb;
  border: 2px dashed #e2e8f0;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #999;
  text-align: center;
}

.export-buttons {
  display: flex;
  gap: 1rem;
}

.performers-table th {
  background: #f9fafb;
  border-bottom: 2px solid #e2e8f0;
}

.performers-table tr:hover {
  background: #f9fafb;
}

@media (max-width: 768px) {
  .date-range {
    flex-direction: column;
    align-items: flex-start;
  }

  .charts-grid {
    grid-template-columns: 1fr;
  }

  .export-buttons {
    flex-direction: column;
  }

  .btn {
    width: 100%;
  }
}
</style>
