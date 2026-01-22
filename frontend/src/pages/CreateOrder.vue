<template>
  <div class="create-order">
    <!-- Verificación de autenticación -->
    <div v-if="!userStore.isAuthenticated" class="auth-required">
      <div class="alert alert-warning">
        <h3>📋 Debes iniciar sesión para crear un encargo</h3>
        <p>Por favor inicia sesión o crea una cuenta para continuar.</p>
        <router-link to="/login" class="btn btn-primary">
          Ir a Iniciar Sesión
        </router-link>
      </div>
    </div>

    <!-- Formulario de creación de encargo -->
    <template v-else>
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
    </template>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/userStore'
import { 
  getTimezoneByState, 
  getTimezoneInfo, 
  convertTimeToTimezone,
  isValidCallTime,
  MEXICAN_STATES
} from '../services/timezones'

const router = useRouter()
const userStore = useUserStore()
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
/* Specific CreateOrder styles only */
.create-order {
  max-width: 600px;
  margin: 0 auto;
}

.auth-required {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 60vh;
}

.price-input {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  padding: 0.75rem;
  width: 100%;
  box-sizing: border-box;
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
  outline: none;
}

.checkbox label {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  cursor: pointer;
}

.checkbox input[type="checkbox"] {
  width: 20px;
  height: 20px;
  cursor: pointer;
  accent-color: var(--primary-color);
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
  color: var(--primary-color);
  font-weight: 600;
}

.word-count-warning {
  background-color: #fff3cd;
  border: 1px solid #ffc107;
}

.word-count-warning .word-count {
  color: #ff6b6b;
}

.timezone-info {
  margin-top: 0.75rem;
  padding: 0.75rem;
  background: #f5f9ff;
  border-left: 3px solid var(--primary-color);
  border-radius: 4px;
}

.timezone-info p {
  margin: 0.25rem 0;
  font-size: 0.9rem;
  color: #333;
}

.time-info {
  margin-top: 0.75rem;
  padding: 0.75rem;
  background: #f5f9ff;
  border-left: 3px solid var(--primary-color);
  border-radius: 4px;
}

.form-actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-top: 2rem;
}

@media (min-width: 640px) {
  .form-actions {
    flex-direction: row;
  }

  .form-actions .btn {
    flex: 1;
  }
}
</style>
