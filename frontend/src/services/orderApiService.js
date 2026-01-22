import apiClient from './apiClient'

export const orderApiService = {
  /**
   * Get all orders for current user
   * @returns {Promise}
   */
  getOrders() {
    return apiClient.get('/api/orders')
  },

  /**
   * Get single order by ID
   * @param {number} orderId
   * @returns {Promise}
   */
  getOrder(orderId) {
    return apiClient.get(`/api/orders/${orderId}`)
  },

  /**
   * Create new order
   * @param {Object} orderData - { message, recipientName, recipientPhone, category, wordCount, totalPrice, ... }
   * @returns {Promise}
   */
  createOrder(orderData) {
    return apiClient.post('/api/orders', orderData)
  },

  /**
   * Update order status
   * @param {number} orderId
   * @param {string} status - pending, accepted, in_progress, completed, cancelled
   * @returns {Promise}
   */
  updateOrderStatus(orderId, status) {
    return apiClient.put(`/api/orders/${orderId}/status`, { status })
  },

  /**
   * Cancel order
   * @param {number} orderId
   * @returns {Promise}
   */
  cancelOrder(orderId) {
    return apiClient.delete(`/api/orders/${orderId}`)
  },

  /**
   * Get available messengers for new order
   * @returns {Promise}
   */
  getMessengers() {
    return apiClient.get('/api/messengers')
  },

  /**
   * Get single messenger details
   * @param {string} messengerId
   * @returns {Promise}
   */
  getMessenger(messengerId) {
    return apiClient.get(`/api/messengers/${messengerId}`)
  },

  /**
   * Get available orders for messenger (to accept)
   * @returns {Promise}
   */
  getAvailableOrders() {
    return apiClient.get('/api/orders?status=pending')
  },

  /**
   * Accept order as messenger
   * @param {number} orderId
   * @returns {Promise}
   */
  acceptOrder(orderId) {
    return apiClient.post(`/api/orders/${orderId}/accept`, {})
  },

  /**
   * Send message for order
   * @param {number} orderId
   * @param {string} content
   * @returns {Promise}
   */
  sendMessage(orderId, content) {
    return apiClient.post(`/api/orders/${orderId}/messages`, { content })
  },

  /**
   * Get messages for order
   * @param {number} orderId
   * @returns {Promise}
   */
  getMessages(orderId) {
    return apiClient.get(`/api/orders/${orderId}/messages`)
  },

  /**
   * Mark message as read
   * @param {number} messageId
   * @returns {Promise}
   */
  markMessageAsRead(messageId) {
    return apiClient.put(`/api/messages/${messageId}/read`, {})
  },

  /**
   * Get call attempts for order
   * @param {number} orderId
   * @returns {Promise}
   */
  getCallAttempts(orderId) {
    return apiClient.get(`/api/calls/${orderId}/attempts`)
  },

  /**
   * Get call recording
   * @param {number} callAttemptId
   * @returns {Promise}
   */
  getCallRecording(callAttemptId) {
    return apiClient.get(`/api/calls/${callAttemptId}/recording`)
  },

  /**
   * Create dispute for order
   * @param {number} orderId
   * @param {string} reason
   * @param {string} description
   * @returns {Promise}
   */
  createDispute(orderId, reason, description) {
    return apiClient.post(`/api/orders/${orderId}/dispute`, {
      reason,
      description,
    })
  },

  /**
   * Get disputes for order
   * @param {number} orderId
   * @returns {Promise}
   */
  getDisputes(orderId) {
    return apiClient.get(`/api/orders/${orderId}/disputes`)
  },
}

export default orderApiService
