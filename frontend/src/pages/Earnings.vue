<template>
  <div class="earnings-page">
    <div class="container">
      <h1>💰 Mis Ganancias</h1>

      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-value">${{ totalEarnings.toFixed(2) }}</div>
          <div class="stat-label">Total Ganado</div>
          <div class="stat-sub">en {{ totalOrders }} órdenes</div>
        </div>
        <div class="stat-card">
          <div class="stat-value">${{ pendingBalance.toFixed(2) }}</div>
          <div class="stat-label">Saldo Disponible</div>
          <div class="stat-sub">listo para retirar</div>
        </div>
        <div class="stat-card">
          <div class="stat-value">${{ avgEarning.toFixed(2) }}</div>
          <div class="stat-label">Ganancia Promedio</div>
          <div class="stat-sub">por orden</div>
        </div>
      </div>

      <div class="section">
        <div class="section-header">
          <h2>Últimas Transacciones</h2>
          <select v-model="filterMonth" class="filter-select">
            <option value="all">Todos los meses</option>
            <option v-for="m in 12" :key="m" :value="m">
              Mes {{ m }}
            </option>
          </select>
        </div>

        <div v-if="filteredTransactions.length === 0" class="empty">
          No hay transacciones
        </div>

        <table v-else class="transactions-table">
          <thead>
            <tr>
              <th>Fecha</th>
              <th>Descripción</th>
              <th>Orden</th>
              <th class="amount">Monto</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="tx in filteredTransactions" :key="tx.id">
              <td>{{ formatDate(tx.date) }}</td>
              <td>{{ tx.description }}</td>
              <td class="order-link">
                <router-link :to="`/orders/${tx.orderId}`">
                  #{{ tx.orderId }}
                </router-link>
              </td>
              <td class="amount">+${{ tx.amount.toFixed(2) }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="section">
        <h2>Retiros</h2>
        <div class="withdraw-card">
          <div class="withdraw-info">
            <p>Saldo disponible para retirar</p>
            <p class="amount">${{ pendingBalance.toFixed(2) }}</p>
          </div>
          <button 
            @click="initiateWithdraw"
            :disabled="pendingBalance === 0"
            class="btn btn-primary"
          >
            💳 Solicitar Retiro
          </button>
        </div>

        <div v-if="withdrawals.length" class="withdrawals-list">
          <h3>Historial de Retiros</h3>
          <div v-for="w in withdrawals" :key="w.id" class="withdrawal-item">
            <div class="withdrawal-date">{{ formatDate(w.date) }}</div>
            <div class="withdrawal-amount">${{ w.amount.toFixed(2) }}</div>
            <div :class="['withdrawal-status', `status-${w.status.toLowerCase()}`]">
              {{ w.status }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const totalEarnings = ref(0)
const totalOrders = ref(0)
const pendingBalance = ref(0)
const filterMonth = ref('all')
const transactions = ref([])
const withdrawals = ref([])

const avgEarning = computed(() => {
  return totalOrders.value > 0 ? totalEarnings.value / totalOrders.value : 0
})

const filteredTransactions = computed(() => {
  if (filterMonth.value === 'all') return transactions.value
  return transactions.value.filter(tx => {
    const month = new Date(tx.date).getMonth() + 1
    return month === parseInt(filterMonth.value)
  })
})

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('es-MX')
}

onMounted(async () => {
  try {
    const response = await fetch('/api/messengers/earnings', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    if (response.ok) {
      const data = await response.json()
      totalEarnings.value = data.totalEarnings
      totalOrders.value = data.totalOrders
      pendingBalance.value = data.pendingBalance
      transactions.value = data.transactions
      withdrawals.value = data.withdrawals
    }
  } catch (error) {
    console.error('Error loading earnings:', error)
  }
})

const initiateWithdraw = async () => {
  const confirmed = confirm(`¿Retirar $${pendingBalance.value.toFixed(2)}?`)
  if (!confirmed) return

  try {
    const response = await fetch('/api/messengers/withdraw', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify({
        amount: pendingBalance.value
      })
    })
    if (response.ok) {
      alert('✅ Solicitud de retiro enviada')
      pendingBalance.value = 0
    }
  } catch (error) {
    console.error('Error requesting withdrawal:', error)
  }
}
</script>

<style scoped>
.earnings-page {
  min-height: 100vh;
  background: #f9fafb;
  padding: 2rem 0;
}

.container {
  max-width: 1000px;
  margin: 0 auto;
  padding: 0 1rem;
}

h1 {
  margin: 0 0 2rem 0;
  font-size: 2.5rem;
  color: #333;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.stat-card {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.stat-value {
  font-size: 2.5rem;
  font-weight: bold;
  color: #667eea;
  margin-bottom: 0.5rem;
}

.stat-label {
  color: #333;
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.stat-sub {
  color: #999;
  font-size: 0.9rem;
}

.section {
  background: white;
  border-radius: 8px;
  padding: 2rem;
  margin-bottom: 2rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.section h2 {
  margin: 0;
  color: #333;
}

.filter-select {
  padding: 0.5rem 1rem;
  border: 1px solid #e2e8f0;
  border-radius: 4px;
  font-size: 0.9rem;
}

.empty {
  text-align: center;
  padding: 2rem;
  color: #999;
}

.transactions-table {
  width: 100%;
  border-collapse: collapse;
}

.transactions-table th {
  background: #f9fafb;
  padding: 1rem;
  text-align: left;
  border-bottom: 2px solid #e2e8f0;
  font-weight: 600;
  color: #666;
}

.transactions-table td {
  padding: 1rem;
  border-bottom: 1px solid #e2e8f0;
  color: #333;
}

.transactions-table th.amount,
.transactions-table td.amount {
  text-align: right;
  color: #10b981;
  font-weight: 600;
}

.order-link a {
  color: #667eea;
  text-decoration: none;
}

.order-link a:hover {
  text-decoration: underline;
}

.withdraw-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2rem;
  border-radius: 8px;
  margin-bottom: 2rem;
}

.withdraw-info p {
  margin: 0;
}

.withdraw-info .amount {
  font-size: 2rem;
  font-weight: bold;
  margin-top: 0.5rem;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-primary {
  background: white;
  color: #667eea;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
}

.withdrawals-list {
  margin-top: 2rem;
}

.withdrawals-list h3 {
  margin: 0 0 1rem 0;
  color: #333;
}

.withdrawal-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 4px;
  margin-bottom: 0.5rem;
}

.withdrawal-date {
  color: #666;
}

.withdrawal-amount {
  font-weight: 600;
  color: #333;
}

.withdrawal-status {
  font-size: 0.85rem;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
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

@media (max-width: 768px) {
  .section-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .withdraw-card {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }
}
</style>
