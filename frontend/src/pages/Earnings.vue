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
/* Specific Earnings page styles only */
.earnings-page {
  background: #f9fafb;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.stat-card {
  border-left: 4px solid var(--primary-color);
}

.transactions-table th {
  background: #f9fafb;
  border-bottom: 2px solid #e2e8f0;
}

.transactions-table th.amount,
.transactions-table td.amount {
  text-align: right;
  color: #10b981;
  font-weight: 600;
}

.order-link a {
  color: var(--primary-color);
  text-decoration: none;
}

.order-link a:hover {
  text-decoration: underline;
}

.withdraw-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: linear-gradient(135deg, var(--primary-color) 0%, #4a3d7a 100%);
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

.btn-primary {
  background: white;
  color: var(--primary-color);
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
}

.withdrawal-item {
  background: #f9fafb;
  margin-bottom: 0.5rem;
}

.withdrawal-date {
  color: #666;
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
  .withdraw-card {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }
}
</style>
