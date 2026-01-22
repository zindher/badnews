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

