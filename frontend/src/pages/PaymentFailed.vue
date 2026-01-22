<template>
  <div class="payment-failed">
    <div class="container">
      <div class="error-card">
        <div class="error-icon">❌</div>
        <h1>Pago Rechazado</h1>
        <p class="message">Hubo un problema al procesar tu pago</p>
        
        <div class="error-details">
          <p class="error-reason">{{ errorMessage }}</p>
        </div>

        <div class="suggestions">
          <h3>Qué puedes hacer:</h3>
          <ul>
            <li>Verifica que los datos de tu tarjeta sean correctos</li>
            <li>Intenta con otra tarjeta de crédito</li>
            <li>Asegúrate de tener saldo disponible</li>
            <li>Contacta a tu banco si persiste el problema</li>
          </ul>
        </div>

        <div class="actions">
          <button @click="retryPayment" class="btn btn-primary">
            🔄 Reintentar Pago
          </button>
          <router-link to="/orders/new" class="btn btn-secondary">
            Cancelar
          </router-link>
        </div>

        <div class="support">
          <p>¿Necesitas ayuda? Contacta a <a href="mailto:support@gritalo.mx">support@gritalo.mx</a></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const errorMessage = ref('Tu tarjeta fue rechazada. Intenta con otra.')

onMounted(() => {
  // Get error from route params
  const error = sessionStorage.getItem('paymentError')
  if (error) {
    errorMessage.value = error
    sessionStorage.removeItem('paymentError')
  }
})

const retryPayment = () => {
  router.back()
}
</script>

<style scoped>
.payment-failed {
  min-height: 100vh;
  background: linear-gradient(135deg, #f87171 0%, #dc2626 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
}

.container {
  width: 100%;
  max-width: 600px;
}

.error-card {
  background: white;
  border-radius: 12px;
  padding: 3rem;
  text-align: center;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.2);
}

.error-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
  display: inline-block;
}

h1 {
  margin: 0 0 0.5rem 0;
  color: #333;
  font-size: 2rem;
}

.message {
  color: #666;
  margin: 0 0 2rem 0;
  font-size: 1.1rem;
}

.error-details {
  background: #fee2e2;
  border-left: 4px solid #dc2626;
  padding: 1.5rem;
  margin: 2rem 0;
  border-radius: 4px;
  text-align: left;
}

.error-reason {
  margin: 0;
  color: #991b1b;
  line-height: 1.6;
}

.suggestions {
  text-align: left;
  background: #fef3c7;
  border-left: 4px solid #f59e0b;
  padding: 1.5rem;
  border-radius: 4px;
  margin: 2rem 0;
}

.suggestions h3 {
  margin: 0 0 1rem 0;
  color: #b45309;
}

.suggestions ul {
  margin: 0;
  padding-left: 1.5rem;
  color: #b45309;
}

.suggestions li {
  margin-bottom: 0.5rem;
  line-height: 1.6;
}

.actions {
  display: flex;
  gap: 1rem;
  margin: 2rem 0;
}

.btn {
  flex: 1;
  padding: 1rem;
  border: none;
  border-radius: 4px;
  font-weight: 600;
  text-decoration: none;
  text-align: center;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 1rem;
}

.btn-primary {
  background: #dc2626;
  color: white;
}

.btn-primary:hover {
  background: #991b1b;
  transform: translateY(-2px);
}

.btn-secondary {
  background: white;
  border: 2px solid #dc2626;
  color: #dc2626;
}

.btn-secondary:hover {
  background: #fee2e2;
}

.support {
  margin-top: 2rem;
  padding-top: 2rem;
  border-top: 1px solid #e2e8f0;
  color: #666;
  font-size: 0.9rem;
}

.support a {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
}

.support a:hover {
  text-decoration: underline;
}

@media (max-width: 640px) {
  .error-card {
    padding: 2rem 1.5rem;
  }

  .actions {
    flex-direction: column;
  }
}
</style>
