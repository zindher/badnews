<template>
  <div class="payment-form">
    <h2>Confirmar Pago</h2>
    
    <div class="summary">
      <div class="summary-item">
        <span>Encargo para {{ recipientName }}</span>
        <span>${{ amount }}</span>
      </div>
      <div class="summary-item">
        <span>Comisión de plataforma</span>
        <span>${{ (amount * 0.1).toFixed(2) }}</span>
      </div>
      <div class="summary-total">
        <span>Total a pagar</span>
        <span>${{ (amount * 1.1).toFixed(2) }}</span>
      </div>
    </div>

    <form @submit.prevent="submit" class="form">
      <div class="form-group">
        <label>Email</label>
        <input 
          v-model="formData.email" 
          type="email" 
          placeholder="tu@email.com"
          required
        />
      </div>

      <div class="form-group">
        <label>Nombre en tarjeta</label>
        <input 
          v-model="formData.cardName" 
          type="text" 
          placeholder="Juan Pérez"
          required
        />
      </div>

      <div class="form-row">
        <div class="form-group">
          <label>Número de tarjeta</label>
          <input 
            v-model="formData.cardNumber" 
            type="text" 
            placeholder="4111 1111 1111 1111"
            maxlength="19"
            @input="formatCardNumber"
            required
          />
        </div>
        <div class="form-group">
          <label>CVV</label>
          <input 
            v-model="formData.cvv" 
            type="text" 
            placeholder="123"
            maxlength="4"
            required
          />
        </div>
      </div>

      <div class="form-row">
        <div class="form-group">
          <label>Exp. Mes</label>
          <select v-model="formData.expMonth" required>
            <option value="">Mes</option>
            <option v-for="m in 12" :key="m" :value="String(m).padStart(2, '0')">
              {{ String(m).padStart(2, '0') }}
            </option>
          </select>
        </div>
        <div class="form-group">
          <label>Exp. Año</label>
          <select v-model="formData.expYear" required>
            <option value="">Año</option>
            <option v-for="y in 15" :key="y" :value="String(2026 + y)">
              {{ 2026 + y }}
            </option>
          </select>
        </div>
      </div>

      <div class="checkbox">
        <input 
          v-model="formData.agreeTerms" 
          type="checkbox" 
          id="agree"
        />
        <label for="agree">
          Acepto los términos y condiciones
        </label>
      </div>

      <button 
        type="submit" 
        class="btn btn-primary"
        :disabled="loading"
      >
        {{ loading ? 'Procesando...' : `Pagar $${(amount * 1.1).toFixed(2)}` }}
      </button>

      <p v-if="error" class="error">{{ error }}</p>
      <p class="secure">🔒 Pago seguro con MercadoPago</p>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const props = defineProps({
  orderId: String,
  recipientName: String,
  amount: Number
})

const emit = defineEmits(['payment-success', 'payment-error'])

const loading = ref(false)
const error = ref(null)

const formData = ref({
  email: '',
  cardName: '',
  cardNumber: '',
  cvv: '',
  expMonth: '',
  expYear: '',
  agreeTerms: false
})

const formatCardNumber = () => {
  let value = formData.value.cardNumber.replace(/\s+/g, '')
  value = value.replace(/(\d{4})/g, '$1 ').trim()
  formData.value.cardNumber = value
}

const submit = async () => {
  if (!formData.value.agreeTerms) {
    error.value = 'Debes aceptar los términos'
    return
  }

  loading.value = true
  error.value = null

  try {
    // TODO: Implementar integración con MercadoPago API
    const response = await fetch('/api/payments/create', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        orderId: props.orderId,
        amount: props.amount,
        email: formData.value.email,
        cardData: {
          cardNumber: formData.value.cardNumber.replace(/\s+/g, ''),
          cardholderName: formData.value.cardName,
          securityCode: formData.value.cvv,
          expirationMonth: parseInt(formData.value.expMonth),
          expirationYear: parseInt(formData.value.expYear)
        }
      })
    })

    if (!response.ok) throw new Error('Payment failed')
    
    const data = await response.json()
    emit('payment-success', data)
  } catch (err) {
    error.value = 'Error al procesar el pago: ' + err.message
    emit('payment-error', err)
  } finally {
    loading.value = false
  }
}

