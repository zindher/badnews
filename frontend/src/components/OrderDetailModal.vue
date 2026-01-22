<template>
  <div class="modal-overlay" @click.self="close">
    <div class="modal">
      <div class="modal-header">
        <h2>Detalles del Encargo</h2>
        <button class="close-btn" @click="close">✕</button>
      </div>

      <div class="modal-body">
        <div class="detail-section">
          <h3>Para:</h3>
          <p class="detail-text">{{ order.recipientName }}</p>
          <p class="detail-text phone">{{ order.recipientPhoneNumber }}</p>
        </div>

        <div class="detail-section">
          <h3>Mensaje:</h3>
          <div class="message-box">{{ order.message }}</div>
          <p class="word-count">{{ wordCount }} palabras | {{ estimatedTime }} min estimado</p>
        </div>

        <div class="detail-section">
          <h3>Información:</h3>
          <div class="info-grid">
            <div class="info-item">
              <span class="label">Tipo:</span>
              <span class="value">{{ getTypeLabel(order.messageType) }}</span>
            </div>
            <div class="info-item">
              <span class="label">Zona horaria:</span>
              <span class="value">{{ order.recipientTimezone }}</span>
            </div>
            <div class="info-item">
              <span class="label">Horario preferido:</span>
              <span class="value">{{ order.preferredCallTime }}</span>
            </div>
            <div class="info-item">
              <span class="label">Precio:</span>
              <span class="value price">${{ order.price }}</span>
            </div>
          </div>
        </div>

        <div v-if="order.isAnonymous" class="anonymous-info">
          <span class="icon">🔒</span>
          <span>El receptor no sabrá quién pagó por esta llamada</span>
        </div>

        <div class="status-section">
          <h3>Estado:</h3>
          <div :class="['status-badge', `status-${order.status.toLowerCase()}`]">
            {{ getStatusLabel(order.status) }}
          </div>
        </div>

        <div v-if="order.callAttempts" class="attempts-section">
          <h3>Intentos:</h3>
          <p>{{ order.callAttempts }} intentos realizados</p>
          <p v-if="order.retryDay" class="retry-info">
            Día {{ order.retryDay }} de reintentos
          </p>
        </div>
      </div>

      <div class="modal-footer">
        <button class="btn btn-secondary" @click="close">Cerrar</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  order: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['close'])

const wordCount = computed(() => {
  return props.order.message.split(/\s+/).filter(w => w).length
})

const estimatedTime = computed(() => {
  return Math.ceil(wordCount.value / 150)
})

const getTypeLabel = (type) => {
  const labels = {
    joke: '😂 Broma',
    confession: '💔 Confesión',
    truth: '✨ Verdad'
  }
  return labels[type] || type
}

const getStatusLabel = (status) => {
  const labels = {
    pending: 'Pendiente',
    inprogress: 'En progreso',
    completed: 'Completado',
    failed: 'Fallido'
  }
  return labels[status.toLowerCase()] || status
}

const close = () => {
  emit('close')
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal {
  background: white;
  border-radius: 8px;
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e2e8f0;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.5rem;
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 1.5rem;
  cursor: pointer;
  transition: transform 0.2s;
}

.close-btn:hover {
  transform: scale(1.2);
}

.modal-body {
  padding: 1.5rem;
}

.detail-section {
  margin-bottom: 2rem;
}

.detail-section h3 {
  margin: 0 0 0.5rem 0;
  color: #333;
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: #666;
}

.detail-text {
  margin: 0;
  color: #333;
  font-size: 1.1rem;
}

.detail-text.phone {
  font-family: monospace;
  font-weight: 500;
  color: #667eea;
}

.message-box {
  background: #f9fafb;
  border-left: 4px solid #667eea;
  padding: 1rem;
  border-radius: 4px;
  line-height: 1.6;
  color: #333;
  margin: 0.5rem 0;
}

.word-count {
  font-size: 0.85rem;
  color: #666;
  margin: 0.5rem 0 0 0;
}

.info-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.info-item {
  display: flex;
  flex-direction: column;
}

.label {
  font-size: 0.75rem;
  text-transform: uppercase;
  color: #666;
  margin-bottom: 0.25rem;
  font-weight: 600;
}

.value {
  color: #333;
  font-weight: 500;
}

.price {
  font-size: 1.25rem;
  color: #667eea;
}

.anonymous-info {
  background: #f0f9ff;
  border-left: 4px solid #3b82f6;
  padding: 0.75rem 1rem;
  border-radius: 4px;
  margin-bottom: 1.5rem;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  color: #1e40af;
  font-size: 0.9rem;
}

.icon {
  font-size: 1.25rem;
}

.status-section {
  margin-bottom: 1.5rem;
}

.status-badge {
  display: inline-block;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
  text-transform: uppercase;
}

.status-pending {
  background: #fef3c7;
  color: #b45309;
}

.status-inprogress {
  background: #dbeafe;
  color: #1e40af;
}

.status-completed {
  background: #dcfce7;
  color: #166534;
}

.status-failed {
  background: #fee2e2;
  color: #991b1b;
}

.attempts-section {
  background: #f9fafb;
  padding: 1rem;
  border-radius: 4px;
  margin-bottom: 1.5rem;
}

.attempts-section h3 {
  margin: 0 0 0.5rem 0;
}

.attempts-section p {
  margin: 0.25rem 0;
  color: #666;
}

.retry-info {
  color: #667eea;
  font-weight: 500;
}

.modal-footer {
  padding: 1.5rem;
  border-top: 1px solid #e2e8f0;
  background: #f9fafb;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: 1px solid #e2e8f0;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
}

.btn:hover {
  background: #f3f4f6;
  border-color: #d1d5db;
}
</style>
