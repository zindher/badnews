<template>
  <div class="admin-dashboard">
    <div class="container">
      <h1>🛠️ Panel Administrativo</h1>

      <div class="stats-grid">
        <div class="stat-box">
          <div class="stat-value">{{ stats.totalUsers }}</div>
          <div class="stat-label">Usuarios</div>
        </div>
        <div class="stat-box">
          <div class="stat-value">{{ stats.totalOrders }}</div>
          <div class="stat-label">Órdenes</div>
        </div>
        <div class="stat-box">
          <div class="stat-value">${{ stats.totalRevenue.toFixed(2) }}</div>
          <div class="stat-label">Ingresos</div>
        </div>
        <div class="stat-box">
          <div class="stat-value">{{ stats.successRate }}%</div>
          <div class="stat-label">Tasa de Éxito</div>
        </div>
      </div>

      <div class="sections">
        <div class="section">
          <h2>Órdenes Recientes</h2>
          <div v-if="recentOrders.length === 0" class="empty">Sin órdenes</div>
          <table v-else class="admin-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Comprador</th>
                <th>Receptor</th>
                <th>Monto</th>
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="order in recentOrders" :key="order.id">
                <td>#{{ order.id }}</td>
                <td>{{ order.buyerName }}</td>
                <td>{{ order.recipientName }}</td>
                <td>${{ order.price.toFixed(2) }}</td>
                <td>
                  <span :class="['status', `status-${order.status.toLowerCase()}`]">
                    {{ order.status }}
                  </span>
                </td>
                <td>
                  <button @click="viewOrder(order.id)" class="action-btn">Ver</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="section">
          <h2>Usuarios Activos</h2>
          <div v-if="activeUsers.length === 0" class="empty">Sin usuarios</div>
          <div v-else class="users-list">
            <div v-for="user in activeUsers" :key="user.id" class="user-item">
              <div class="user-info">
                <div class="user-name">{{ user.name }}</div>
                <div class="user-email">{{ user.email }}</div>
              </div>
              <div class="user-stats">
                <span>{{ user.orders }} órdenes</span>
                <button @click="manageUser(user.id)" class="action-btn">Gestionar</button>
              </div>
            </div>
          </div>
        </div>

        <div class="section">
          <h2>Reportes de Problemas</h2>
          <div v-if="disputes.length === 0" class="empty">Sin disputas</div>
          <div v-else class="disputes-list">
            <div v-for="dispute in disputes" :key="dispute.id" class="dispute-item">
              <div class="dispute-header">
                <div class="dispute-title">{{ dispute.title }}</div>
                <span :class="['priority', `priority-${dispute.priority}`]">
                  {{ dispute.priority }}
                </span>
              </div>
              <p class="dispute-desc">{{ dispute.description }}</p>
              <div class="dispute-actions">
                <button @click="resolveDispute(dispute.id)" class="action-btn">Resolver</button>
                <button @click="contactUser(dispute.userId)" class="action-btn">Contactar</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const stats = ref({
  totalUsers: 0,
  totalOrders: 0,
  totalRevenue: 0,
  successRate: 0
})

const recentOrders = ref([])
const activeUsers = ref([])
const disputes = ref([])

onMounted(async () => {
  try {
    const response = await fetch('/api/admin/dashboard', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    if (response.ok) {
      const data = await response.json()
      stats.value = data.stats
      recentOrders.value = data.recentOrders
      activeUsers.value = data.activeUsers
      disputes.value = data.disputes
    }
  } catch (error) {
    console.error('Error loading admin dashboard:', error)
  }
})

const viewOrder = (orderId) => {
  // Navigate to order details
  console.log('View order', orderId)
}

const manageUser = (userId) => {
  // Open user management modal
  console.log('Manage user', userId)
}

const resolveDispute = (disputeId) => {
  // Mark dispute as resolved
  console.log('Resolve dispute', disputeId)
}

const contactUser = (userId) => {
  // Open contact user dialog
  console.log('Contact user', userId)
}
</script>

<style scoped>
.admin-dashboard {
  min-height: 100vh;
  background: #f9fafb;
  padding: 2rem 0;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

h1 {
  margin: 0 0 2rem 0;
  font-size: 2rem;
  color: #333;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.stat-box {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.stat-value {
  font-size: 2rem;
  font-weight: bold;
  color: #667eea;
}

.stat-label {
  color: #666;
  font-size: 0.9rem;
  margin-top: 0.5rem;
}

.sections {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.section {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.section h2 {
  margin: 0 0 1.5rem 0;
  color: #333;
}

.empty {
  padding: 2rem;
  text-align: center;
  color: #999;
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
}

.admin-table th {
  background: #f9fafb;
  padding: 1rem;
  text-align: left;
  border-bottom: 2px solid #e2e8f0;
  font-weight: 600;
  color: #666;
}

.admin-table td {
  padding: 1rem;
  border-bottom: 1px solid #e2e8f0;
}

.status {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 600;
}

.status-pending {
  background: #fef3c7;
  color: #b45309;
}

.status-completed {
  background: #dcfce7;
  color: #166534;
}

.status-failed {
  background: #fee2e2;
  color: #991b1b;
}

.users-list {
  space-y: 1rem;
}

.user-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 4px;
  margin-bottom: 0.5rem;
}

.user-info {
  flex: 1;
}

.user-name {
  font-weight: 600;
  color: #333;
}

.user-email {
  font-size: 0.85rem;
  color: #666;
}

.user-stats {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.disputes-list {
  space-y: 1rem;
}

.dispute-item {
  padding: 1rem;
  background: #f9fafb;
  border-radius: 4px;
  margin-bottom: 1rem;
  border-left: 4px solid #667eea;
}

.dispute-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.dispute-title {
  font-weight: 600;
  color: #333;
}

.priority {
  font-size: 0.8rem;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-weight: 600;
}

.priority-high {
  background: #fee2e2;
  color: #991b1b;
}

.priority-medium {
  background: #fef3c7;
  color: #b45309;
}

.priority-low {
  background: #dbeafe;
  color: #1e40af;
}

.dispute-desc {
  margin: 0.5rem 0;
  color: #666;
  font-size: 0.9rem;
}

.dispute-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 0.75rem;
}

.action-btn {
  padding: 0.5rem 1rem;
  border: 1px solid #e2e8f0;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.85rem;
  font-weight: 600;
  transition: all 0.2s;
}

.action-btn:hover {
  background: #f3f4f6;
  border-color: #667eea;
  color: #667eea;
}
</style>
