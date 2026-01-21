import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useOrderStore = defineStore('order', () => {
  const orders = ref([])
  const currentOrder = ref(null)
  const loading = ref(false)
  const error = ref(null)

  const totalOrders = computed(() => orders.value.length)
  const completedOrders = computed(() => 
    orders.value.filter(o => o.status === 'Completed').length
  )

  const setOrders = (newOrders) => {
    orders.value = newOrders
  }

  const addOrder = (order) => {
    orders.value.push(order)
  }

  const setCurrentOrder = (order) => {
    currentOrder.value = order
  }

  const setLoading = (value) => {
    loading.value = value
  }

  const setError = (message) => {
    error.value = message
  }

  return {
    orders,
    currentOrder,
    loading,
    error,
    totalOrders,
    completedOrders,
    setOrders,
    addOrder,
    setCurrentOrder,
    setLoading,
    setError
  }
})
