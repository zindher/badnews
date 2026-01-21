import axios from 'axios'

const API_BASE_URL = '/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

export const OrderService = {
  createOrder(orderData) {
    return apiClient.post('/orders', orderData)
  },
  
  getAvailableOrders() {
    return apiClient.get('/orders/available')
  },
  
  updateOrderStatus(orderId, status) {
    return apiClient.put(`/orders/${orderId}/status`, { status })
  }
}

export default apiClient
