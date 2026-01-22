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
.analytics-page {
  min-height: 100vh;
  background: #f9fafb;
  padding: 2rem 0;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

h1 {
  margin: 0 0 2rem 0;
  font-size: 2rem;
  color: #333;
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

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-primary:hover {
  transform: translateY(-2px);
}

.btn-secondary {
  background: white;
  border: 1px solid #e2e8f0;
  color: #666;
}

.btn-secondary:hover {
  background: #f9fafb;
}

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.metric-card {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.metric-title {
  color: #666;
  font-size: 0.9rem;
  margin-bottom: 0.5rem;
}

.metric-value {
  font-size: 2rem;
  font-weight: bold;
  color: #333;
  margin-bottom: 0.5rem;
}

.metric-change {
  font-size: 0.9rem;
  font-weight: 600;
}

.metric-change.positive {
  color: #10b981;
}

.metric-change.negative {
  color: #ef4444;
}

.metric-subtitle {
  font-size: 0.85rem;
  color: #999;
  margin-top: 0.5rem;
}

.charts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.chart-card {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.chart-card h3 {
  margin: 0 0 1.5rem 0;
  color: #333;
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

.export-section {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  margin-bottom: 2rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.export-section h3 {
  margin: 0 0 1rem 0;
  color: #333;
}

.export-buttons {
  display: flex;
  gap: 1rem;
}

.top-performers {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.top-performers h3 {
  margin: 0 0 1.5rem 0;
  color: #333;
}

.performers-table {
  width: 100%;
  border-collapse: collapse;
}

.performers-table th {
  background: #f9fafb;
  padding: 1rem;
  text-align: left;
  border-bottom: 2px solid #e2e8f0;
  font-weight: 600;
  color: #666;
}

.performers-table td {
  padding: 1rem;
  border-bottom: 1px solid #e2e8f0;
  color: #333;
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
