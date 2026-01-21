<template>
  <div class="create-order">
    <h2>Crear Nuevo Encargo</h2>
    
    <form @submit.prevent="submitForm" class="order-form">
      <div class="form-group">
        <label for="recipientName">Nombre del Receptor:</label>
        <input 
          v-model="form.recipientName" 
          type="text" 
          id="recipientName" 
          required
          placeholder="Ej: Juan"
        >
      </div>

      <div class="form-group">
        <label for="recipientPhone">Teléfono del Receptor:</label>
        <input 
          v-model="form.recipientPhoneNumber" 
          type="tel" 
          id="recipientPhone" 
          required
          placeholder="Ej: +52 555 123 4567"
        >
      </div>

      <div class="form-group">
        <label for="recipientEmail">Email del Receptor (Opcional - para notificación fallback):</label>
        <input 
          v-model="form.recipientEmail" 
          type="email" 
          id="recipientEmail" 
          placeholder="Ej: juan@example.com"
        >
        <p class="help-text">Si proporcionas email, recibirá una notificación si no podemos contactarlo por teléfono</p>
      </div>

      <div class="form-group">
        <label for="state">Estado del Receptor:</label>
        <select 
          v-model="form.recipientState" 
          id="state" 
          required
          @change="onStateChange"
        >
          <option value="">Selecciona un estado...</option>
          <option v-for="state in mexicanStates" :key="state" :value="state">
            {{ state }}
          </option>
        </select>
        <div v-if="recipientTimezone" class="timezone-info">
          <p><strong>Zona Horaria:</strong> {{ recipientTimezone.name }}</p>
        </div>
      </div>

      <div class="form-group">
        <label for="preferredTime">Mejor Horario para Llamar:</label>
        <div class="time-input-wrapper">
          <input 
            v-model="form.preferredCallTime" 
            type="time" 
            id="preferredTime" 
            required
            @change="onTimeChange"
          >
          <div v-if="form.preferredCallTime" class="time-info">
            <p class="time-label">Hora en Aguascalientes: <strong>{{ form.preferredCallTime }}</strong></p>
            <p v-if="convertedTime" class="time-label">
              Hora en zona del receptor: <strong>{{ convertedTime.time }} {{ convertedTime.dayLabel }}</strong>
            </p>
            <p v-if="timeValidationError" class="error-inline">{{ timeValidationError }}</p>
          </div>
        </div>
      </div>

      <div class="form-group">
        <label for="message">Mensaje (Máximo 250 palabras):</label>
        <textarea 
          v-model="form.message" 
          id="message" 
          rows="5" 
          required
          placeholder="Escribe aquí qué quieres que diga el mensajero..."
          @input="onMessageChange"
        ></textarea>
        <div class="message-counter" :class="{ 'word-count-warning': wordCount > 250 }">
          <span class="word-count">{{ wordCount }}/250 palabras</span>
          <span v-if="wordCount > 250" class="error-inline">⚠️ Excedes el límite de 250 palabras</span>
        </div>
        <p class="help-text">Aproximadamente {{ estimatedTime }} minutos de duración</p>
      </div>

      <div class="form-group">
        <label for="price">Precio:</label>
        <div class="price-input">
          <span>$</span>
          <input 
            v-model.number="form.price" 
            type="number" 
            id="price" 
            step="0.01" 
            min="100"
            required
          >
          <span>MXN</span>
        </div>
      </div>

      <div class="form-group checkbox">
        <label for="anonymous">
          <input 
            v-model="form.isAnonymous" 
            type="checkbox" 
            id="anonymous"
          >
          Encargo Anónimo
        </label>
        <p class="help-text">Si marcas esto, el mensajero no sabrá quién solicitó la llamada</p>
      </div>

      <div class="form-actions">
        <button type="submit" class="btn btn-primary" :disabled="loading">
          {{ loading ? 'Creando...' : 'Crear Encargo' }}
        </button>
        <router-link to="/orders" class="btn btn-secondary">Cancelar</router-link>
      </div>
    </form>

    <div v-if="error" class="error-message">{{ error }}</div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { OrderService } from '../services/orderService'
import { 
  getTimezoneByState, 
  getTimezoneInfo, 
  convertTimeToTimezone,
  isValidCallTime,
  MEXICAN_STATES
} from '../services/timezones'

const router = useRouter()
const loading = ref(false)
const error = ref(null)
const recipientTimezone = ref(null)
const convertedTime = ref(null)
const timeValidationError = ref(null)
const mexicanStates = ref(MEXICAN_STATES)
const wordCount = ref(0)

const form = ref({
  recipientName: '',
  recipientPhoneNumber: '',
  recipientEmail: '', // Email opcional para fallback
  recipientState: '', // Estado del receptor
  message: '',
  price: 199, // Precio único
  isAnonymous: false,
  preferredCallTime: '', // HH:MM formato
  buyerId: 'user123' // TODO: Get from auth
})

/**
 * Calcula el tiempo estimado basado en palabras
 * Velocidad: ~150 palabras/minuto en español
 */
const estimatedTime = computed(() => {
  const minutes = Math.ceil(wordCount.value / 150)
  return minutes > 0 ? minutes : '0-1'
})

/**
 * Calcula el número de palabras en el mensaje
 */
const onMessageChange = () => {
  const words = form.value.message.trim().split(/\s+/).filter(word => word.length > 0)
  wordCount.value = words.length
}

/**
 * Al cambiar el estado, detecta la zona horaria
 */
const onStateChange = () => {
  if (form.value.recipientState) {
    const timezoneKey = getTimezoneByState(form.value.recipientState)
    recipientTimezone.value = getTimezoneInfo(timezoneKey)
    
    // Si ya hay hora preferida, recalcular la conversión
    if (form.value.preferredCallTime) {
      onTimeChange()
    }
  } else {
    recipientTimezone.value = null
    convertedTime.value = null
  }
}

/**
 * Al cambiar la hora preferida, valida y convierte a zona destino
 */
const onTimeChange = () => {
  timeValidationError.value = null
  convertedTime.value = null
  
  if (!form.value.preferredCallTime) return
  
  // Validar que no sea después de las 21:00
  if (!isValidCallTime(form.value.preferredCallTime)) {
    timeValidationError.value = '⚠️ El horario máximo permitido es 21:00 (9 PM)'
    return
  }
  
  // Convertir hora a zona del receptor
  if (recipientTimezone.value) {
    const timezoneKey = getTimezoneByState(form.value.recipientState)
    convertedTime.value = convertTimeToTimezone(form.value.preferredCallTime, timezoneKey)
  }
}

const submitForm = async () => {
  // Validar que se haya seleccionado estado
  if (!form.value.recipientState) {
    error.value = 'Por favor selecciona el estado del receptor'
    return
  }

  // Validar que la hora sea válida SI se proporciona
  if (form.value.preferredCallTime && !isValidCallTime(form.value.preferredCallTime)) {
    error.value = 'Por favor selecciona un horario válido (máximo 21:00)'
    return
  }

  // Validar número de palabras
  if (wordCount.value > 250) {
    error.value = '⚠️ El mensaje no puede exceder 250 palabras'
    return
  }

  if (wordCount.value < 10) {
    error.value = 'El mensaje debe tener al menos 10 palabras'
    return
  }

  loading.value = true
  error.value = null

  try {
    // Detectar zona horaria del receptor
    const timezoneKey = getTimezoneByState(form.value.recipientState)
    
    // Preparar datos a enviar
    const orderData = {
      recipientPhoneNumber: form.value.recipientPhoneNumber,
      recipientName: form.value.recipientName,
      recipientEmail: form.value.recipientEmail || null, // Opcional
      recipientState: form.value.recipientState,
      message: form.value.message,
      price: form.value.price,
      isAnonymous: form.value.isAnonymous,
      preferredCallTime: form.value.preferredCallTime || null,
      buyerId: form.value.buyerId
    }
    
    await OrderService.createOrder(orderData)
    router.push('/orders')
  } catch (err) {
    error.value = 'Error al crear el encargo. Intenta de nuevo.'
    console.error(err)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.create-order {
  width: 100%;
  max-width: 600px;
  margin: 0 auto;
}

h2 {
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
}

.order-form {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #333;
  font-size: 0.95rem;
}

input[type="text"],
input[type="tel"],
input[type="time"],
input[type="number"],
select,
textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 1rem;
  font-family: inherit;
  transition: border-color 0.3s, box-shadow 0.3s;
  box-sizing: border-box;
  -webkit-appearance: none;
  appearance: none;
}

select {
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='8' viewBox='0 0 12 8'%3E%3Cpath fill='%23667eea' d='M6 8L0 0h12z'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 0.75rem center;
  padding-right: 2.5rem;
  cursor: pointer;
}

input[type="text"]:focus,
input[type="tel"]:focus,
input[type="time"]:focus,
input[type="number"]:focus,
select:focus,
textarea:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

textarea {
  resize: vertical;
  min-height: 120px;
}

.price-input {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  padding: 0.75rem;
}

.price-input span {
  color: #666;
  font-weight: 500;
}

.price-input input {
  flex: 1;
  border: none;
  padding: 0;
  margin: 0;
}

.price-input input:focus {
  box-shadow: none;
  border-color: transparent;
}

.checkbox label {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0;
  cursor: pointer;
}

.checkbox input[type="checkbox"] {
  width: 20px;
  height: 20px;
  cursor: pointer;
  accent-color: #667eea;
}

.help-text {
  margin: 0.5rem 0 0 0;
  color: #666;
  font-size: 0.85rem;
  line-height: 1.4;
}

.message-counter {
  margin-top: 0.5rem;
  padding: 0.75rem;
  background-color: #f5f5f5;
  border-radius: 4px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.85rem;
}

.word-count {
  color: #667eea;
  font-weight: 600;
}

.word-count-warning {
  background-color: #fff3cd;
  border: 1px solid #ffc107;
}

.word-count-warning .word-count {
  color: #ff6b6b;
}

.error-inline {
  color: #ff6b6b;
  font-weight: 600;
  margin: 0;
}

.form-actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-top: 2rem;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  text-decoration: none;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 44px;
  touch-action: manipulation;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-primary:active:not(:disabled) {
  transform: scale(0.98);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-secondary {
  background: #f0f0f0;
  color: #333;
  border: 1px solid #ddd;
}

.btn-secondary:active {
  transform: scale(0.98);
}

.error-message {
  color: #d32f2f;
  background: #ffebee;
  padding: 1rem;
  border-radius: 6px;
  margin-top: 1rem;
  border-left: 4px solid #d32f2f;
}

.timezone-info {
  margin-top: 0.75rem;
  padding: 0.75rem;
  background: #f5f9ff;
  border-left: 3px solid #667eea;
  border-radius: 4px;
}

.timezone-info p {
  margin: 0.25rem 0;
  font-size: 0.9rem;
  color: #333;
}

.timezone-hint {
  color: #666;
  font-size: 0.85rem;
  font-style: italic;
}

.time-input-wrapper {
  width: 100%;
}

.time-info {
  margin-top: 0.75rem;
  padding: 0.75rem;
  background: #f5f9ff;
  border-left: 3px solid #667eea;
  border-radius: 4px;
}

.time-label {
  margin: 0.25rem 0;
  font-size: 0.9rem;
  color: #333;
}

.error-inline {
  color: #d32f2f;
  background: #ffebee;
  padding: 0.5rem;
  border-radius: 3px;
  margin-top: 0.5rem;
  font-size: 0.85rem;
}

@media (min-width: 640px) {
  h2 {
    font-size: 1.8rem;
  }

  .order-form {
    padding: 2rem;
  }

  .form-actions {
    flex-direction: row;
  }

  .btn {
    flex: 1;
  }
}

@media (min-width: 1024px) {
  h2 {
    font-size: 2rem;
  }

  .btn-primary:hover:not(:disabled) {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
  }

  .btn-secondary:hover {
    background: #e0e0e0;
  }

  .btn:active {
    transform: scale(1);
  }
}
</style>
