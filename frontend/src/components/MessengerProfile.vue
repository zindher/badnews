<template>
  <div class="messenger-profile">
    <div class="profile-header">
      <div class="avatar">{{ initials }}</div>
      <div class="header-info">
        <h2>{{ messenger.name }}</h2>
        <div class="rating">
          <span class="stars">★ {{ messenger.rating.toFixed(1) }}</span>
          <span class="count">({{ messenger.totalCompletedOrders }} órdenes)</span>
        </div>
        <div :class="['availability', messenger.isAvailable ? 'available' : 'unavailable']">
          {{ messenger.isAvailable ? '🟢 Disponible' : '⚫ No disponible' }}
        </div>
      </div>
    </div>

    <div class="profile-stats">
      <div class="stat">
        <div class="stat-value">${{ messenger.totalEarnings.toFixed(2) }}</div>
        <div class="stat-label">Ingresos Totales</div>
      </div>
      <div class="stat">
        <div class="stat-value">${{ messenger.pendingBalance.toFixed(2) }}</div>
        <div class="stat-label">Saldo Pendiente</div>
      </div>
      <div class="stat">
        <div class="stat-value">{{ messenger.totalCompletedOrders }}</div>
        <div class="stat-label">Órdenes Completadas</div>
      </div>
    </div>

    <div class="profile-actions">
      <button 
        @click="toggleAvailability"
        :class="['btn', messenger.isAvailable ? 'btn-warning' : 'btn-success']"
        :disabled="loading"
      >
        {{ messenger.isAvailable ? '⚫ Desactivar' : '🟢 Activar' }}
      </button>
      <button class="btn btn-secondary" @click="withdrawFunds" :disabled="messenger.pendingBalance === 0">
        💳 Retirar Fondos
      </button>
    </div>

    <div class="profile-sections">
      <div class="section">
        <h3>Información Personal</h3>
        <div class="info-item">
          <span class="label">Nombre:</span>
          <span class="value">{{ messenger.name }}</span>
        </div>
        <div class="info-item">
          <span class="label">Email:</span>
          <span class="value">{{ messenger.email }}</span>
        </div>
        <div class="info-item">
          <span class="label">Teléfono:</span>
          <span class="value">{{ messenger.phone }}</span>
        </div>
      </div>

      <div class="section">
        <h3>Rendimiento</h3>
        <div class="performance">
          <div class="perf-item">
            <span class="label">Tasa de éxito:</span>
            <span class="value">{{ successRate }}%</span>
          </div>
          <div class="perf-item">
            <span class="label">Tiempo promedio:</span>
            <span class="value">{{ messenger.avgResponseTime }} horas</span>
          </div>
          <div class="perf-item">
            <span class="label">Se unió:</span>
            <span class="value">{{ formatDate(messenger.createdAt) }}</span>
          </div>
        </div>
      </div>

      <div class="section">
        <h3>Historial Reciente</h3>
        <div v-if="messenger.recentOrders && messenger.recentOrders.length" class="recent-list">
          <div v-for="order in messenger.recentOrders.slice(0, 3)" :key="order.id" class="recent-item">
            <span class="name">{{ order.recipientName }}</span>
            <span :class="['status', `status-${order.status.toLowerCase()}`]">
              {{ order.status }}
            </span>
          </div>
        </div>
        <p v-else class="empty">Sin órdenes recientes</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  messenger: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['toggle-availability', 'withdraw'])

const loading = ref(false)

const initials = computed(() => {
  const names = props.messenger.name.split(' ')
  return (names[0]?.[0] || '') + (names[1]?.[0] || '')
})

const successRate = computed(() => {
  if (props.messenger.totalCompletedOrders === 0) return 0
  // Assuming we have a failedOrders property
  const success = props.messenger.totalCompletedOrders / (props.messenger.totalCompletedOrders + (props.messenger.failedOrders || 0))
  return Math.round(success * 100)
})

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('es-MX')
}

const toggleAvailability = async () => {
  loading.value = true
  try {
    emit('toggle-availability', !props.messenger.isAvailable)
  } finally {
    loading.value = false
  }
}

const withdrawFunds = () => {
  emit('withdraw', props.messenger.pendingBalance)
}
</script>

<style scoped>
.messenger-profile {
  background: white;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.profile-header {
  display: flex;
  gap: 1.5rem;
  padding: 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  align-items: center;
}

.avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  font-weight: bold;
  border: 3px solid white;
}

.header-info h2 {
  margin: 0 0 0.5rem 0;
  font-size: 1.75rem;
}

.rating {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
  font-size: 0.95rem;
}

.stars {
  font-weight: bold;
}

.count {
  opacity: 0.9;
}

.availability {
  font-size: 0.9rem;
  font-weight: 600;
  display: inline-block;
}

.availability.available {
  color: #4ade80;
}

.availability.unavailable {
  color: #ef4444;
}

.profile-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
  padding: 2rem;
  background: #f9fafb;
  border-bottom: 1px solid #e2e8f0;
}

.stat {
  text-align: center;
}

.stat-value {
  font-size: 1.75rem;
  font-weight: bold;
  color: #667eea;
}

.stat-label {
  font-size: 0.85rem;
  color: #666;
  margin-top: 0.5rem;
}

.profile-actions {
  display: flex;
  gap: 1rem;
  padding: 1.5rem 2rem;
  border-bottom: 1px solid #e2e8f0;
}

.btn {
  flex: 1;
  padding: 0.75rem 1rem;
  border: none;
  border-radius: 4px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-success {
  background: #10b981;
  color: white;
}

.btn-success:hover:not(:disabled) {
  background: #059669;
}

.btn-warning {
  background: #f59e0b;
  color: white;
}

.btn-warning:hover:not(:disabled) {
  background: #d97706;
}

.btn-secondary {
  background: white;
  border: 1px solid #e2e8f0;
  color: #666;
}

.btn-secondary:hover:not(:disabled) {
  background: #f9fafb;
}

.profile-sections {
  padding: 2rem;
}

.section {
  margin-bottom: 2rem;
}

.section h3 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 1.1rem;
  border-bottom: 2px solid #667eea;
  padding-bottom: 0.5rem;
}

.info-item, .perf-item {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem 0;
  border-bottom: 1px solid #e2e8f0;
}

.info-item:last-child, .perf-item:last-child {
  border-bottom: none;
}

.label {
  color: #666;
  font-weight: 500;
}

.value {
  color: #333;
  font-weight: 600;
}

.performance {
  background: #f9fafb;
  padding: 1rem;
  border-radius: 4px;
}

.recent-list {
  space-y: 0.5rem;
}

.recent-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background: #f9fafb;
  border-radius: 4px;
  margin-bottom: 0.5rem;
}

.recent-item:last-child {
  margin-bottom: 0;
}

.name {
  font-weight: 500;
  color: #333;
}

.status {
  font-size: 0.8rem;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-weight: 600;
  text-transform: uppercase;
}

.status-completed {
  background: #dcfce7;
  color: #166534;
}

.status-inprogress {
  background: #dbeafe;
  color: #1e40af;
}

.status-pending {
  background: #fef3c7;
  color: #b45309;
}

.empty {
  color: #999;
  font-size: 0.9rem;
  margin: 0;
  padding: 1rem;
  text-align: center;
  background: #f9fafb;
  border-radius: 4px;
}

@media (max-width: 768px) {
  .profile-header {
    flex-direction: column;
    text-align: center;
  }

  .profile-stats {
    grid-template-columns: 1fr;
  }

  .profile-actions {
    flex-direction: column;
  }

  .btn {
    width: 100%;
  }
}
</style>
