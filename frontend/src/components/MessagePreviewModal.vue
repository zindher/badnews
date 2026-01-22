<template>
  <div class="message-preview">
    <div class="preview-header">
      <h2>Vista previa del mensaje</h2>
      <button class="close-btn" @click="close">✕</button>
    </div>

    <div class="preview-body">
      <div class="preview-card">
        <div class="card-header">
          <div>
            <h3>{{ recipientName }}</h3>
            <p class="subtitle">Mensaje de {{ messageType }}</p>
          </div>
          <div class="duration">${{ price }}</div>
        </div>

        <div class="card-body">
          <div class="message-content">
            <p>{{ message }}</p>
          </div>

          <div class="meta-info">
            <div class="meta-item">
              <span class="icon">📝</span>
              <span>{{ wordCount }} palabras</span>
            </div>
            <div class="meta-item">
              <span class="icon">⏱️</span>
              <span>~{{ estimatedDuration }} minutos</span>
            </div>
            <div class="meta-item">
              <span class="icon">🕐</span>
              <span>{{ preferredCallTime }}</span>
            </div>
            <div v-if="isAnonymous" class="meta-item anonymous">
              <span class="icon">🔒</span>
              <span>Anónimo</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="preview-footer">
      <button class="btn btn-secondary" @click="close">Editar</button>
      <button class="btn btn-primary" @click="confirm">Confirmar</button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  recipientName: String,
  messageType: String,
  message: String,
  price: Number,
  preferredCallTime: String,
  isAnonymous: Boolean
})

const emit = defineEmits(['close', 'confirm'])

const wordCount = computed(() => {
  return props.message.split(/\s+/).filter(w => w).length
})

const estimatedDuration = computed(() => {
  return Math.ceil(wordCount.value / 150)
})

const close = () => {
  emit('close')
}

const confirm = () => {
  emit('confirm')
}
</script>

<style scoped>
/* Specific MessagePreviewModal component styles only */
.message-preview {
  padding: 1rem;
}

.preview-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  width: 100%;
  max-width: 600px;
}

.preview-header h2 {
  margin: 0;
  color: white;
  font-size: 1.5rem;
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 2rem;
  cursor: pointer;
  padding: 0;
}

.preview-body {
  width: 100%;
  max-width: 600px;
  max-height: 70vh;
  overflow-y: auto;
}

.preview-card {
  overflow: hidden;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 1.5rem;
  background: linear-gradient(135deg, var(--primary-color) 0%, #4a3d7a 100%);
  color: white;
}

.card-header h3 {
  margin: 0 0 0.5rem 0;
  font-size: 1.25rem;
}

.subtitle {
  margin: 0;
  font-size: 0.9rem;
  opacity: 0.9;
}

.duration {
  background: rgba(255, 255, 255, 0.2);
  padding: 0.5rem 1rem;
  border-radius: 4px;
  font-size: 1.25rem;
  font-weight: bold;
}

.message-content {
  background: #f9fafc;
  padding: 1rem;
  border-radius: 6px;
  border-left: 4px solid var(--primary-color);
  margin-bottom: 1.5rem;
  line-height: 1.6;
  color: #333;
}

.message-content p {
  margin: 0;
}

.meta-info {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem;
  background: #f9fafb;
  border-radius: 4px;
  color: #666;
  font-size: 0.9rem;
}

.meta-item.anonymous {
  background: #dbeafe;
  color: #1e40af;
  grid-column: 1 / -1;
}

.preview-footer {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
  width: 100%;
  max-width: 600px;
}

.btn-primary {
  background: linear-gradient(135deg, var(--primary-color) 0%, #4a3d7a 100%);
  color: white;
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(91, 75, 159, 0.4);
}

.btn-secondary {
  background: white;
  border: 1px solid #e2e8f0;
  color: #666;
}

.btn-secondary:hover {
  background: #f9fafb;
}
</style>
