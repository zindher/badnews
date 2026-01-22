<template>
  <div class="messenger-card">
    <div class="card-header">
      <div class="order-info">
        <h3>{{ order.recipientName }}</h3>
        <p class="recipient-phone">📞 {{ maskPhone(order.recipientPhoneNumber) }}</p>
      </div>
      <div class="price-badge">${{ order.price }}</div>
    </div>

    <div class="card-body">
      <div class="message-preview">
        <p class="label">Mensaje:</p>
        <p class="message-text">{{ truncateMessage(order.message) }}</p>
      </div>

      <div class="order-details">
        <div class="detail">
          <span class="label">Tipo:</span>
          <span class="value">{{ capitalizeType(order.messageType) }}</span>
        </div>
        <div class="detail">
          <span class="label">Duración estimada:</span>
          <span class="value">{{ estimatedDuration }} min</span>
        </div>
        <div class="detail">
          <span class="label">Zona:</span>
          <span class="value">{{ order.recipientTimezone }}</span>
        </div>
        <div class="detail">
          <span class="label">Horario:</span>
          <span class="value">{{ order.preferredCallTime }}</span>
        </div>
      </div>

      <div v-if="order.isAnonymous" class="anonymous-badge">
        🔒 Comprador anónimo
      </div>
    </div>

    <div class="card-footer">
      <button 
        @click="decline" 
        class="btn btn-secondary"
        :disabled="loading"
      >
        Rechazar
      </button>
      <button 
        @click="accept" 
        class="btn btn-primary"
        :disabled="loading"
      >
        {{ loading ? 'Aceptando...' : 'Aceptar' }}
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  order: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['accept', 'decline'])
const loading = ref(false)

const estimatedDuration = computed(() => {
  const wordCount = props.order.message.split(/\s+/).filter(w => w).length
  return Math.ceil(wordCount / 150) // ~150 words per minute
})

const truncateMessage = (msg) => {
  if (!msg) return ''
  return msg.length > 100 ? msg.substring(0, 100) + '...' : msg
}

const maskPhone = (phone) => {
  if (!phone) return ''
  return phone.replace(/(\d{2})(\d{3})(\d{4})/, '+$1 $2 $3')
}

const capitalizeType = (type) => {
  const types = {
    joke: '😂 Broma',
    confession: '💔 Confesión',
    truth: '✨ Verdad'
  }
  return types[type] || type
}

const accept = async () => {
  loading.value = true
  try {
    emit('accept', props.order.id)
  } finally {
    loading.value = false
  }
}

const decline = () => {
  emit('decline', props.order.id)
}
</script>

<style scoped>
/* Specific MessengerCard component styles only */
.messenger-card {
  transition: transform 0.2s, box-shadow 0.2s;
}

.messenger-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 1rem;
  background: linear-gradient(135deg, var(--primary-color) 0%, #4a3d7a 100%);
  color: white;
}

.order-info h3 {
  margin: 0 0 0.5rem 0;
  font-size: 1.25rem;
  font-weight: 600;
}

.recipient-phone {
  margin: 0;
  font-size: 0.9rem;
  opacity: 0.9;
}

.price-badge {
  background: rgba(255, 255, 255, 0.2);
  padding: 0.5rem 1rem;
  border-radius: 4px;
  font-size: 1.5rem;
  font-weight: bold;
}

.message-preview {
  margin-bottom: 1.5rem;
}

.label {
  display: block;
  font-size: 0.75rem;
  text-transform: uppercase;
  color: #666;
  margin-bottom: 0.25rem;
  font-weight: 600;
  letter-spacing: 0.5px;
}

.message-text {
  margin: 0;
  font-style: italic;
  color: #333;
  line-height: 1.5;
  background: #f7fafc;
  padding: 0.75rem;
  border-radius: 4px;
  border-left: 3px solid var(--primary-color);
}

.order-details {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-bottom: 1rem;
}

.detail {
  display: flex;
  flex-direction: column;
}

.detail .value {
  color: #333;
  font-weight: 500;
}

.anonymous-badge {
  background: #f3f4f6;
  border: 1px solid #d1d5db;
  padding: 0.5rem 0.75rem;
  border-radius: 4px;
  font-size: 0.9rem;
  text-align: center;
  color: #666;
  margin-bottom: 1rem;
}

.card-footer {
  display: flex;
  gap: 0.5rem;
  padding: 1rem;
  background: #f9fafb;
  border-top: 1px solid #e2e8f0;
}

.btn-primary {
  background: linear-gradient(135deg, var(--primary-color) 0%, #4a3d7a 100%);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(91, 75, 159, 0.4);
}

.btn-secondary {
  background: white;
  border: 1px solid #e2e8f0;
  color: #666;
}

.btn-secondary:hover:not(:disabled) {
  background: #f9fafb;
  border-color: #cbd5e1;
}

@media (max-width: 640px) {
  .card-header {
    flex-direction: column;
    gap: 0.5rem;
  }

  .order-details {
    grid-template-columns: 1fr;
  }

  .card-footer {
    flex-direction: column;
  }
}
</style>
