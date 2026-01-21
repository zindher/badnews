<template>
  <div class="orders">
    <h2>Mis Encargos</h2>
    
    <div v-if="loading" class="loading">Cargando...</div>
    <div v-else-if="orders.length === 0" class="empty-state">
      <p>No tienes encargos aún</p>
      <router-link to="/orders/new" class="btn btn-primary">Crear Encargo</router-link>
    </div>
    <div v-else class="orders-list">
      <div v-for="order in orders" :key="order.id" class="order-card">
        <div class="order-header">
          <span class="status" :class="order.status.toLowerCase()">{{ order.status }}</span>
          <span class="anonymous" v-if="order.isAnonymous">🔒 Anónimo</span>
        </div>
        <p><strong>Receptor:</strong> {{ order.recipientName }}</p>
        <p><strong>Teléfono:</strong> {{ order.recipientPhoneNumber }}</p>
        <p><strong>Mensaje:</strong> {{ order.message.substring(0, 100) }}...</p>
        <p><strong>Precio:</strong> ${{ order.price }} MXN</p>
        <div v-if="order.callRecordingUrl" class="recording">
          <strong>🎥 Grabación disponible</strong>
          <a :href="order.callRecordingUrl" target="_blank">Escuchar</a>
        </div>
        <div v-if="order.status === 'Completed' && !order.rating" class="rating-section">
          <p>¿Cómo fue tu experiencia?</p>
          <div class="stars">
            <button 
              v-for="star in 5" 
              :key="star" 
              @click="rateOrder(order.id, star)"
              :class="{ active: star <= hoveredRating }"
            >
              ⭐
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { OrderService } from '../services/orderService'
import { useOrderStore } from '../stores/orderStore'

const orderStore = useOrderStore()
const orders = ref([])
const loading = ref(true)
const hoveredRating = ref(0)

onMounted(async () => {
  try {
    const response = await OrderService.getAvailableOrders()
    orders.value = response.data
  } catch (error) {
    console.error('Error fetching orders:', error)
  } finally {
    loading.value = false
  }
})

const rateOrder = (orderId, rating) => {
  const order = orders.value.find(o => o.id === orderId)
  if (order) {
    order.rating = rating
    // TODO: Send rating to API
  }
}
</script>

<style scoped>
.orders {
  animation: fadeIn 0.5s ease-in;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

h2 {
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
}

.loading {
  text-align: center;
  padding: 2rem 1rem;
  color: #666;
}

.empty-state {
  text-align: center;
  padding: 2rem 1rem;
  color: #666;
}

.empty-state p {
  margin-bottom: 1rem;
}

.empty-state .btn {
  display: inline-block;
}

.orders-list {
  display: grid;
  gap: 1rem;
}

.order-card {
  background: white;
  padding: 1rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border-left: 4px solid #667eea;
}

.order-header {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  align-items: center;
  flex-wrap: wrap;
}

.status {
  display: inline-block;
  padding: 0.4rem 0.8rem;
  border-radius: 4px;
  font-size: 0.8rem;
  font-weight: bold;
  color: white;
  white-space: nowrap;
}

.status.pending {
  background: #ff9800;
}

.status.assigned {
  background: #2196f3;
}

.status.inprogress {
  background: #9c27b0;
}

.status.completed {
  background: #4caf50;
}

.status.failed {
  background: #f44336;
}

.anonymous {
  font-size: 0.8rem;
  color: #667eea;
  background: #f0f0ff;
  padding: 0.4rem 0.8rem;
  border-radius: 4px;
}

.order-card p {
  margin: 0.5rem 0;
  color: #333;
  font-size: 0.95rem;
  word-break: break-word;
}

.order-card strong {
  color: #667eea;
}

.recording {
  margin-top: 1rem;
  padding: 1rem;
  background: #f5f5f5;
  border-radius: 6px;
  border-left: 3px solid #4caf50;
}

.recording strong {
  display: block;
  margin-bottom: 0.5rem;
}

.recording a {
  color: #667eea;
  text-decoration: none;
  font-weight: bold;
  display: inline-block;
  padding: 0.5rem 1rem;
  background: white;
  border-radius: 4px;
  border: 1px solid #667eea;
}

.rating-section {
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid #eee;
}

.rating-section p {
  margin-bottom: 0.75rem;
  font-size: 0.95rem;
}

.stars {
  display: flex;
  gap: 0.5rem;
}

.stars button {
  background: none;
  border: none;
  font-size: 2rem;
  cursor: pointer;
  opacity: 0.2;
  transition: opacity 0.2s;
  padding: 0.25rem;
  min-width: 44px;
  min-height: 44px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.stars button:active {
  transform: scale(1.2);
}

.stars button.active {
  opacity: 1;
}

.btn {
  padding: 0.75rem 1.5rem;
  border-radius: 6px;
  text-decoration: none;
  font-weight: bold;
  transition: all 0.3s;
  border: none;
  cursor: pointer;
  min-height: 44px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.btn-primary {
  background: #667eea;
  color: white;
}

.btn-primary:active {
  transform: scale(0.98);
}

/* TABLET */
@media (min-width: 640px) {
  h2 {
    font-size: 1.8rem;
  }

  .orders-list {
    gap: 1.5rem;
  }

  .order-card {
    padding: 1.5rem;
  }

  .order-header {
    gap: 1rem;
  }
}

/* DESKTOP */
@media (min-width: 1024px) {
  h2 {
    font-size: 2rem;
    margin-bottom: 2rem;
  }

  .orders-list {
    gap: 2rem;
  }

  .order-card {
    padding: 2rem;
  }

  .order-card:hover {
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
  }

  .recording a:hover {
    background: #667eea;
    color: white;
  }

  .stars button:active {
    transform: scale(1);
  }

  .stars button:hover {
    opacity: 0.6;
  }
}
</style>
