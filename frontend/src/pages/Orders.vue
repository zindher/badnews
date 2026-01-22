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
/* Specific Orders page styles only */
.orders {
  animation: fadeIn 0.5s ease-in;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.order-card {
  border-left: 4px solid var(--primary-color);
}

.status.pending { background: #ff9800; }
.status.assigned { background: #2196f3; }
.status.inprogress { background: #9c27b0; }
.status.completed { background: #4caf50; }
.status.failed { background: #f44336; }

.recording {
  margin-top: var(--spacing-md);
  padding: var(--spacing-sm);
  background: var(--bg-gray);
  border-left: 3px solid #4caf50;
}

.recording a {
  color: var(--primary-color);
  border: 1px solid var(--primary-color);
}

.recording a:hover {
  background: var(--primary-color);
  color: white;
}

.stars button:active { transform: scale(1.2); }
.stars button:hover { opacity: 0.6; }
</style>
