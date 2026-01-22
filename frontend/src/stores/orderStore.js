import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { orderApiService } from '../services/orderApiService'

export const useOrderStore = defineStore('order', () => {
  // State
  const orders = ref([])
  const currentOrder = ref(null)
  const availableOrders = ref([])
  const messengers = ref([])
  const selectedMessenger = ref(null)
  const isLoading = ref(false)
  const error = ref(null)
  const filters = ref({
    status: null,
    dateRange: null,
    searchTerm: ''
  })

  // Computed
  const filteredOrders = computed(() => {
    let filtered = orders.value
    
    if (filters.value.status) {
      filtered = filtered.filter(o => o.status === filters.value.status)
    }
    
    if (filters.value.searchTerm) {
      const term = filters.value.searchTerm.toLowerCase()
      filtered = filtered.filter(o => 
        o.recipientName?.toLowerCase().includes(term) ||
        o.recipientPhone?.includes(term)
      )
    }
    
    return filtered
  })

  const totalOrders = computed(() => orders.value.length)
  const completedOrders = computed(() => 
    orders.value.filter(o => o.status === 'Completed').length
  )
  const pendingOrders = computed(() =>
    orders.value.filter(o => o.status === 'Pending').length
  )

  // Actions
  const fetchOrders = async () => {
    isLoading.value = true
    error.value = null
    try {
      const data = await orderApiService.getOrders()
      orders.value = data
    } catch (err) {
      error.value = err.message || 'Failed to fetch orders'
      console.error('Fetch orders error:', err)
    } finally {
      isLoading.value = false
    }
  }

  const getOrder = async (orderId) => {
    isLoading.value = true
    error.value = null
    try {
      const data = await orderApiService.getOrder(orderId)
      currentOrder.value = data
      return data
    } catch (err) {
      error.value = err.message || 'Failed to fetch order'
      console.error('Get order error:', err)
      return null
    } finally {
      isLoading.value = false
    }
  }

  const createOrder = async (orderData) => {
    isLoading.value = true
    error.value = null
    try {
      const newOrder = await orderApiService.createOrder(orderData)
      orders.value.push(newOrder)
      currentOrder.value = newOrder
      return newOrder
    } catch (err) {
      error.value = err.message || 'Failed to create order'
      console.error('Create order error:', err)
      return null
    } finally {
      isLoading.value = false
    }
  }

  const updateOrderStatus = async (orderId, status) => {
    isLoading.value = true
    error.value = null
    try {
      const updated = await orderApiService.updateOrderStatus(orderId, status)
      const index = orders.value.findIndex(o => o.id === orderId)
      if (index !== -1) {
        orders.value[index] = updated
      }
      if (currentOrder.value?.id === orderId) {
        currentOrder.value = updated
      }
      return updated
    } catch (err) {
      error.value = err.message || 'Failed to update order status'
      console.error('Update order status error:', err)
      return null
    } finally {
      isLoading.value = false
    }
  }

  const cancelOrder = async (orderId) => {
    isLoading.value = true
    error.value = null
    try {
      const result = await orderApiService.cancelOrder(orderId)
      const index = orders.value.findIndex(o => o.id === orderId)
      if (index !== -1) {
        orders.value.splice(index, 1)
      }
      if (currentOrder.value?.id === orderId) {
        currentOrder.value = null
      }
      return result
    } catch (err) {
      error.value = err.message || 'Failed to cancel order'
      console.error('Cancel order error:', err)
      return false
    } finally {
      isLoading.value = false
    }
  }

  const fetchMessengers = async () => {
    isLoading.value = true
    error.value = null
    try {
      const data = await orderApiService.getMessengers()
      messengers.value = data
    } catch (err) {
      error.value = err.message || 'Failed to fetch messengers'
      console.error('Fetch messengers error:', err)
    } finally {
      isLoading.value = false
    }
  }

  const getMessenger = async (messengerId) => {
    isLoading.value = true
    error.value = null
    try {
      const data = await orderApiService.getMessenger(messengerId)
      selectedMessenger.value = data
      return data
    } catch (err) {
      error.value = err.message || 'Failed to fetch messenger'
      console.error('Get messenger error:', err)
      return null
    } finally {
      isLoading.value = false
    }
  }

  const fetchAvailableOrders = async () => {
    isLoading.value = true
    error.value = null
    try {
      const data = await orderApiService.getAvailableOrders()
      availableOrders.value = data
    } catch (err) {
      error.value = err.message || 'Failed to fetch available orders'
      console.error('Fetch available orders error:', err)
    } finally {
      isLoading.value = false
    }
  }

  const acceptOrder = async (orderId) => {
    isLoading.value = true
    error.value = null
    try {
      const result = await orderApiService.acceptOrder(orderId)
      const index = availableOrders.value.findIndex(o => o.id === orderId)
      if (index !== -1) {
        availableOrders.value.splice(index, 1)
      }
      orders.value.push(result)
      return result
    } catch (err) {
      error.value = err.message || 'Failed to accept order'
      console.error('Accept order error:', err)
      return null
    } finally {
      isLoading.value = false
    }
  }

  const sendMessage = async (orderId, content) => {
    isLoading.value = true
    error.value = null
    try {
      const message = await orderApiService.sendMessage(orderId, content)
      return message
    } catch (err) {
      error.value = err.message || 'Failed to send message'
      console.error('Send message error:', err)
      return null
    } finally {
      isLoading.value = false
    }
  }

  const getMessages = async (orderId) => {
    isLoading.value = true
    error.value = null
    try {
      const messages = await orderApiService.getMessages(orderId)
      return messages
    } catch (err) {
      error.value = err.message || 'Failed to fetch messages'
      console.error('Get messages error:', err)
      return []
    } finally {
      isLoading.value = false
    }
  }

  const getCallAttempts = async (orderId) => {
    isLoading.value = true
    error.value = null
    try {
      const attempts = await orderApiService.getCallAttempts(orderId)
      return attempts
    } catch (err) {
      error.value = err.message || 'Failed to fetch call attempts'
      console.error('Get call attempts error:', err)
      return []
    } finally {
      isLoading.value = false
    }
  }

  const getCallRecording = async (callAttemptId) => {
    isLoading.value = true
    error.value = null
    try {
      const recording = await orderApiService.getCallRecording(callAttemptId)
      return recording
    } catch (err) {
      error.value = err.message || 'Failed to fetch call recording'
      console.error('Get call recording error:', err)
      return null
    } finally {
      isLoading.value = false
    }
  }

  const setOrders = (newOrders) => {
    orders.value = newOrders
  }

  const addOrder = (order) => {
    orders.value.push(order)
  }

  const setCurrentOrder = (order) => {
    currentOrder.value = order
  }

  const setFilters = (newFilters) => {
    filters.value = { ...filters.value, ...newFilters }
  }

  const clearFilters = () => {
    filters.value = {
      status: null,
      dateRange: null,
      searchTerm: ''
    }
  }

  return {
    // State
    orders,
    currentOrder,
    availableOrders,
    messengers,
    selectedMessenger,
    isLoading,
    error,
    filters,

    // Computed
    filteredOrders,
    totalOrders,
    completedOrders,
    pendingOrders,

    // Actions
    fetchOrders,
    getOrder,
    createOrder,
    updateOrderStatus,
    cancelOrder,
    fetchMessengers,
    getMessenger,
    fetchAvailableOrders,
    acceptOrder,
    sendMessage,
    getMessages,
    getCallAttempts,
    getCallRecording,
    setOrders,
    addOrder,
    setCurrentOrder,
    setFilters,
    clearFilters
  }
})
