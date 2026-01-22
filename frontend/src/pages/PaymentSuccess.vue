<template>
  <div class="payment-success">
    <div class="container">
      <div class="success-card">
        <div class="success-icon">✅</div>
        <h1>¡Pago Confirmado!</h1>
        <p class="message">Tu encargo ha sido creado exitosamente</p>
        
        <div class="order-info">
          <div class="info-item">
            <span class="label">Para:</span>
            <span class="value">{{ orderData.recipientName }}</span>
          </div>
          <div class="info-item">
            <span class="label">Precio:</span>
            <span class="value">${{ orderData.price }}</span>
          </div>
          <div class="info-item">
            <span class="label">Estado:</span>
            <span class="value">Asignando mensajero...</span>
          </div>
        </div>

        <div class="next-steps">
          <h3>Próximos pasos:</h3>
          <ol>
            <li>Te asignaremos un mensajero dentro de 24 horas</li>
            <li>Recibirás notificación por email cuando se confirme</li>
            <li>El mensajero realizará la llamada en el horario especificado</li>
            <li>Recibirás la grabación en tu cuenta</li>
          </ol>
        </div>

        <div class="actions">
          <router-link to="/orders" class="btn btn-primary">
            Ver Mis Encargos
          </router-link>
          <router-link to="/" class="btn btn-secondary">
            Crear Otro
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const orderData = ref({
  recipientName: '',
  price: 0
})

onMounted(() => {
  // Get order data from session storage or route params
  const data = sessionStorage.getItem('lastOrder')
  if (data) {
    orderData.value = JSON.parse(data)
    sessionStorage.removeItem('lastOrder')
  }
})
</script>

<style scoped>
/* Specific PaymentSuccess page styles only */
.payment-success {
  background: linear-gradient(135deg, var(--primary-color) 0%, #4a3d7a 100%);
}

.success-card {
  text-align: center;
}

.success-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
  display: inline-block;
  animation: bounce 0.6s ease-in-out;
}

@keyframes bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-20px); }
}

.order-info {
  background: #f9fafb;
  border-radius: 8px;
  padding: 1.5rem;
  margin: 2rem 0;
  text-align: left;
}

.info-item {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem 0;
  border-bottom: 1px solid #e2e8f0;
}

.info-item:last-child {
  border-bottom: none;
}

.label {
  color: #666;
  font-weight: 500;
}

.value {
  color: #333;
  font-weight: 600;
}

.next-steps {
  text-align: left;
  margin: 2rem 0;
  background: #f0fdf4;
  border-left: 4px solid #10b981;
  padding: 1.5rem;
  border-radius: 4px;
}

.next-steps h3 {
  margin: 0 0 1rem 0;
  color: #166534;
}

.next-steps ol {
  margin: 0;
  padding-left: 1.5rem;
  color: #166534;
}

.next-steps li {
  margin-bottom: 0.5rem;
  line-height: 1.6;
}

.actions {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
}

.btn-primary {
  background: linear-gradient(135deg, var(--primary-color) 0%, #4a3d7a 100%);
  color: white;
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.btn-secondary {
  background: white;
  border: 2px solid var(--primary-color);
  color: var(--primary-color);
}

.btn-secondary:hover {
  background: #f9fafb;
}

@media (max-width: 640px) {
  .actions {
    flex-direction: column;
  }
}
</style>
