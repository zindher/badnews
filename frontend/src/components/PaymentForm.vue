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
</script>

<style scoped>
.payment-form {
  background: white;
  border-radius: 8px;
  padding: 2rem;
  max-width: 500px;
  margin: 0 auto;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

h2 {
  margin-top: 0;
  margin-bottom: 1.5rem;
  color: #333;
}

.summary {
  background: #f9fafb;
  border-radius: 6px;
  padding: 1rem;
  margin-bottom: 2rem;
  border: 1px solid #e2e8f0;
}

.summary-item {
  display: flex;
  justify-content: space-between;
  padding: 0.5rem 0;
  color: #666;
}

.summary-total {
  display: flex;
  justify-content: space-between;
  padding: 1rem 0;
  border-top: 2px solid #e2e8f0;
  font-weight: 600;
  color: #333;
  font-size: 1.1rem;
}

.form-group {
  margin-bottom: 1rem;
}

label {
  display: block;
  font-size: 0.9rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: #333;
}

input, select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #e2e8f0;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.2s;
}

input:focus, select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-bottom: 1rem;
}

.checkbox {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin: 1rem 0;
  font-size: 0.9rem;
}

.checkbox input {
  width: auto;
  margin: 0;
}

.checkbox label {
  margin: 0;
  font-weight: 400;
}

.btn {
  width: 100%;
  padding: 1rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 4px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 1rem;
  margin-top: 1rem;
}

.btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error {
  color: #dc2626;
  font-size: 0.9rem;
  margin: 1rem 0 0 0;
  padding: 0.75rem;
  background: #fee2e2;
  border-radius: 4px;
}

.secure {
  text-align: center;
  color: #666;
  font-size: 0.85rem;
  margin: 1rem 0 0 0;
}
</style>
