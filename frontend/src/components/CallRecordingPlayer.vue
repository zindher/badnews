<template>
  <div class="recording-player">
    <div class="player-wrapper">
      <video 
        ref="videoElement"
        :src="recordingUrl"
        controls
        controlsList="nodownload"
        class="video-player"
        @play="onPlay"
        @pause="onPause"
      ></video>
    </div>

    <div class="player-info">
      <div class="info-item">
        <span class="label">Duración:</span>
        <span class="value">{{ formatDuration(duration) }}</span>
      </div>
      <div class="info-item">
        <span class="label">Descargada:</span>
        <span class="value">{{ new Date(downloadDate).toLocaleDateString() }}</span>
      </div>
    </div>

    <div class="player-actions">
      <button @click="downloadVideo" class="btn btn-primary">
        📥 Descargar grabación
      </button>
      <button @click="shareVideo" class="btn btn-secondary">
        📤 Compartir
      </button>
    </div>

    <div v-if="error" class="error-message">
      {{ error }}
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const props = defineProps({
  recordingUrl: String,
  downloadDate: String
})

const emit = defineEmits(['download', 'share'])

const videoElement = ref(null)
const duration = ref(0)
const error = ref(null)

const formatDuration = (seconds) => {
  const minutes = Math.floor(seconds / 60)
  const secs = Math.floor(seconds % 60)
  return `${minutes}:${secs.toString().padStart(2, '0')}`
}

const onPlay = () => {
  if (videoElement.value) {
    duration.value = videoElement.value.duration
  }
}

const onPause = () => {
  // Optional pause handling
}

const downloadVideo = async () => {
  try {
    emit('download')
    const a = document.createElement('a')
    a.href = props.recordingUrl
    a.download = `grabacion_${Date.now()}.mp4`
    document.body.appendChild(a)
    a.click()
    document.body.removeChild(a)
  } catch (err) {
    error.value = 'Error al descargar: ' + err.message
  }
}

const shareVideo = () => {
  emit('share')
  if (navigator.share) {
    navigator.share({
      title: 'Mira mi grabación de Gritalo',
      text: '¡Revive el momento!',
      url: props.recordingUrl
    })
  } else {
    // Fallback: copy to clipboard
    navigator.clipboard.writeText(props.recordingUrl)
    alert('URL copiada al portapapeles')
  }
}
</script>

<style scoped>
/* Specific CallRecordingPlayer component styles only */
.recording-player {
  overflow: hidden;
}

.player-wrapper {
  position: relative;
  width: 100%;
  padding-bottom: 56.25%; /* 16:9 */
  background: #000;
}

.video-player {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

.player-info {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  padding: 1rem;
  background: #f9fafb;
  border-bottom: 1px solid #e2e8f0;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.label {
  font-size: 0.75rem;
  text-transform: uppercase;
  color: #666;
  font-weight: 600;
  letter-spacing: 0.5px;
}

.value {
  color: #333;
  font-weight: 500;
}

.player-actions {
  display: flex;
  gap: 0.5rem;
  padding: 1rem;
  background: white;
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

.error-message {
  padding: 1rem;
  background: #fee2e2;
  color: #991b1b;
  border-top: 1px solid #fecaca;
  font-size: 0.9rem;
}

@media (max-width: 640px) {
  .player-info {
    grid-template-columns: 1fr;
  }

  .player-actions {
    flex-direction: column;
  }
}
</style>
