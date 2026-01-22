<template>
  <div class="messenger-home">
    <div class="container">
      <div class="header">
        <h1>Órdenes Disponibles</h1>
        <div class="filters">
          <button 
            @click="filterStatus = 'all'"
            :class="['filter-btn', filterStatus === 'all' && 'active']"
          >
            Todas
          </button>
          <button 
            @click="filterStatus = 'pending'"
            :class="['filter-btn', filterStatus === 'pending' && 'active']"
          >
            Pendientes
          </button>
          <button 
            @click="filterStatus = 'high-price'"
            :class="['filter-btn', filterStatus === 'high-price' && 'active']"
          >
            Mayor precio
          </button>
        </div>
      </div>

      <div v-if="loading" class="loading">Cargando órdenes...</div>
      
      <div v-else-if="filteredOrders.length === 0" class="empty">
        <p>No hay órdenes disponibles en este momento</p>
        <button class="btn btn-secondary" @click="checkAgain">Verificar de nuevo</button>
      </div>

      <div v-else class="orders-grid">
        <MessengerCard 
          v-for="order in filteredOrders"
          :key="order.id"
          :order="order"
          @accept="acceptOrder"
          @decline="declineOrder"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import MessengerCard from '../components/MessengerCard.vue'

const orders = ref([])
const loading = ref(false)
const filterStatus = ref('all')

const filteredOrders = computed(() => {
  let filtered = orders.value
  
  if (filterStatus.value === 'pending') {
    filtered = filtered.filter(o => o.status === 'Pending')
  } else if (filterStatus.value === 'high-price') {
    filtered = filtered.sort((a, b) => b.price - a.price)
  }
  
  return filtered
})

onMounted(async () => {
  loading.value = true
  try {
    const response = await fetch('/api/orders/available', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    if (response.ok) {
      const data = await response.json()
      orders.value = data.data || []
    }
  } catch (error) {
    console.error('Error loading orders:', error)
  } finally {
    loading.value = false
  }
})

const acceptOrder = async (orderId) => {
  try {
    const response = await fetch(`/api/orders/${orderId}/accept`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    if (response.ok) {
      // Remove from list
      orders.value = orders.value.filter(o => o.id !== orderId)
      alert('✅ Orden aceptada!')
    }
  } catch (error) {
    console.error('Error accepting order:', error)
  }
}

const declineOrder = (orderId) => {
  // Just remove from display, don't make API call
  orders.value = orders.value.filter(o => o.id !== orderId)
}

const checkAgain = () => {
  location.reload()
}
</script>

<style scoped>
.messenger-home {
  min-height: 100vh;
  background: #f9fafb;
  padding: 2rem 0;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  flex-wrap: wrap;
  gap: 1rem;
}

.header h1 {
  margin: 0;
  font-size: 2rem;
  color: #333;
}

.filters {
  display: flex;
  gap: 0.5rem;
}

.filter-btn {
  padding: 0.5rem 1rem;
  border: 1px solid #e2e8f0;
  background: white;
  border-radius: 20px;
  cursor: pointer;
  transition: all 0.2s;
  font-weight: 500;
  font-size: 0.9rem;
}

.filter-btn:hover {
  border-color: #667eea;
}

.filter-btn.active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-color: transparent;
}

.loading {
  text-align: center;
  padding: 3rem;
  color: #666;
  font-size: 1.1rem;
}

.empty {
  text-align: center;
  padding: 3rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.empty p {
  margin: 0 0 1rem 0;
  color: #666;
  font-size: 1.1rem;
}

.orders-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
}

@media (max-width: 768px) {
  .header {
    flex-direction: column;
    align-items: flex-start;
  }

  .orders-grid {
    grid-template-columns: 1fr;
  }
}
</style>
