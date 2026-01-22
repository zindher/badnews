<template>
  <div class="history-page">
    <div class="container">
      <h1>📞 Historial de Llamadas</h1>

      <div class="filters">
        <select v-model="filterStatus" class="filter">
          <option value="">Todos los estados</option>
          <option value="completed">Completadas</option>
          <option value="failed">Fallidas</option>
          <option value="inprogress">En progreso</option>
        </select>
        <input 
          v-model="searchQuery"
          type="text"
          placeholder="Buscar por nombre..."
          class="search"
        />
      </div>

      <div v-if="loading" class="loading">Cargando historial...</div>
      
      <div v-else-if="filteredCalls.length === 0" class="empty">
        No hay llamadas para mostrar
      </div>

      <div v-else class="calls-table">
        <div class="table-header">
          <div class="col col-date">Fecha</div>
          <div class="col col-recipient">Destinatario</div>
          <div class="col col-status">Estado</div>
          <div class="col col-duration">Duración</div>
          <div class="col col-recording">Grabación</div>
          <div class="col col-amount">Monto</div>
        </div>

        <div v-for="call in filteredCalls" :key="call.id" class="table-row">
          <div class="col col-date">{{ formatDate(call.date) }}</div>
          <div class="col col-recipient">{{ call.recipientName }}</div>
          <div class="col col-status">
            <span :class="['status-badge', `status-${call.status.toLowerCase()}`]">
              {{ getStatusLabel(call.status) }}
            </span>
          </div>
          <div class="col col-duration">{{ call.duration }}s</div>
          <div class="col col-recording">
            <button 
              v-if="call.recordingUrl"
              @click="openRecording(call)"
              class="btn-small btn-primary"
            >
              🎬 Ver
            </button>
            <span v-else class="no-recording">-</span>
          </div>
          <div class="col col-amount">${{ call.amount.toFixed(2) }}</div>
        </div>
      </div>
    </div>

    <RecordingModal 
      v-if="selectedRecording"
      :recording="selectedRecording"
      @close="selectedRecording = null"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import RecordingModal from '../components/CallRecordingPlayer.vue'

const calls = ref([])
const loading = ref(false)
const filterStatus = ref('')
const searchQuery = ref('')
const selectedRecording = ref(null)

const filteredCalls = computed(() => {
  let filtered = calls.value

  if (filterStatus.value) {
    filtered = filtered.filter(c => c.status.toLowerCase() === filterStatus.value)
  }

  if (searchQuery.value) {
    filtered = filtered.filter(c => 
      c.recipientName.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  return filtered
})

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('es-MX')
}

const getStatusLabel = (status) => {
  const labels = {
    completed: '✅ Completada',
    failed: '❌ Fallida',
    inprogress: '⏳ En progreso'
  }
  return labels[status.toLowerCase()] || status
}

onMounted(async () => {
  loading.value = true
  try {
    const response = await fetch('/api/calls/history', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    if (response.ok) {
      const data = await response.json()
      calls.value = data.data || []
    }
  } catch (error) {
    console.error('Error loading call history:', error)
  } finally {
    loading.value = false
  }
})

const openRecording = (call) => {
  selectedRecording.value = call
}
</script>

<style scoped>
.history-page {
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

.filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
}

.filter, .search {
  padding: 0.75rem 1rem;
  border: 1px solid #e2e8f0;
  border-radius: 4px;
  font-size: 0.95rem;
}

.search {
  flex: 1;
  min-width: 200px;
}

.loading {
  text-align: center;
  padding: 3rem;
  color: #666;
}

.empty {
  text-align: center;
  padding: 3rem;
  background: white;
  border-radius: 8px;
  color: #666;
}

.calls-table {
  background: white;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.table-header {
  display: grid;
  grid-template-columns: 120px 200px 120px 100px 120px 100px;
  gap: 1rem;
  padding: 1rem;
  background: #f9fafb;
  border-bottom: 2px solid #e2e8f0;
  font-weight: 600;
  color: #666;
  font-size: 0.9rem;
}

.table-row {
  display: grid;
  grid-template-columns: 120px 200px 120px 100px 120px 100px;
  gap: 1rem;
  padding: 1rem;
  border-bottom: 1px solid #e2e8f0;
  align-items: center;
}

.table-row:hover {
  background: #f9fafb;
}

.col {
  word-break: break-word;
}

.col-date {
  color: #666;
}

.col-recipient {
  color: #333;
  font-weight: 500;
}

.col-status {
}

.col-duration {
  text-align: right;
}

.col-recording {
  text-align: center;
}

.col-amount {
  text-align: right;
  font-weight: 600;
  color: #667eea;
}

.status-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 600;
}

.status-completed {
  background: #dcfce7;
  color: #166534;
}

.status-failed {
  background: #fee2e2;
  color: #991b1b;
}

.status-inprogress {
  background: #dbeafe;
  color: #1e40af;
}

.btn-small {
  padding: 0.4rem 0.8rem;
  border: none;
  border-radius: 4px;
  font-size: 0.8rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-small.btn-primary {
  background: #667eea;
  color: white;
}

.btn-small.btn-primary:hover {
  background: #5568d3;
}

.no-recording {
  color: #999;
}

@media (max-width: 1024px) {
  .table-header, .table-row {
    grid-template-columns: 100px 150px 100px 80px 100px 80px;
  }
}

@media (max-width: 768px) {
  .filters {
    flex-direction: column;
  }

  .table-header, .table-row {
    display: none;
  }

  .calls-table {
    display: block;
  }

  .table-row {
    display: block;
    border-bottom: 2px solid #e2e8f0;
    margin-bottom: 1rem;
  }

  .col {
    display: flex;
    justify-content: space-between;
    padding: 0.5rem 0;
  }

  .col::before {
    content: attr(data-label);
    font-weight: 600;
  }
}
</style>
