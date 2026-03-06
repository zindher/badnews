import apiClient from './apiClient'

export const paymentService = {
  /**
   * Create payment for order (Mercado Pago)
   * @param {number} orderId
   * @returns {Promise}
   */
  createPayment(orderId) {
    return apiClient.post(`/api/payments/${orderId}`, {}).then(r => r.data)
  },

  /**
   * Get payment status
   * @param {number} paymentId
   * @returns {Promise}
   */
  getPaymentStatus(paymentId) {
    return apiClient.get(`/api/payments/${paymentId}`).then(r => r.data)
  },

  /**
   * Get all payments for user
   * @returns {Promise}
   */
  getPayments() {
    return apiClient.get('/api/payments').then(r => r.data)
  },

  /**
   * Refund payment (Admin only)
   * @param {number} paymentId
   * @returns {Promise}
   */
  refundPayment(paymentId) {
    return apiClient.post(`/api/payments/${paymentId}/refund`, {}).then(r => r.data)
  },

  /**
   * Get payment methods
   * @returns {Promise}
   */
  getPaymentMethods() {
    return apiClient.get('/api/payments/methods', {}).then(r => r.data)
  },

  /**
   * Process payment webhook from Mercado Pago
   * @param {Object} webhookData
   * @returns {Promise}
   */
  processWebhook(webhookData) {
    return apiClient.post('/api/payments/webhook', webhookData).then(r => r.data)
  },
}

export const withdrawalService = {
  /**
   * Request withdrawal (Messenger only)
   * @param {number} amount
   * @param {string} bankAccount
   * @param {string} bankName
   * @returns {Promise}
   */
  requestWithdrawal(amount, bankAccount, bankName) {
    return apiClient.post('/api/withdrawals', {
      amount,
      bankAccount,
      bankName,
    }).then(r => r.data)
  },

  /**
   * Get withdrawal requests for messenger
   * @returns {Promise}
   */
  getWithdrawals() {
    return apiClient.get('/api/withdrawals').then(r => r.data)
  },

  /**
   * Get single withdrawal details
   * @param {number} withdrawalId
   * @returns {Promise}
   */
  getWithdrawal(withdrawalId) {
    return apiClient.get(`/api/withdrawals/${withdrawalId}`).then(r => r.data)
  },

  /**
   * Get earnings summary
   * @returns {Promise}
   */
  getEarnings() {
    return apiClient.get('/api/messengers/me/earnings').then(r => r.data)
  },

  /**
   * Get earnings history/transactions
   * @returns {Promise}
   */
  getEarningsHistory() {
    return apiClient.get('/api/messengers/me/earnings/history').then(r => r.data)
  },

  /**
   * Approve withdrawal (Admin only)
   * @param {number} withdrawalId
   * @returns {Promise}
   */
  approveWithdrawal(withdrawalId) {
    return apiClient.put(`/api/withdrawals/${withdrawalId}/approve`, {}).then(r => r.data)
  },

  /**
   * Reject withdrawal (Admin only)
   * @param {number} withdrawalId
   * @param {string} reason
   * @returns {Promise}
   */
  rejectWithdrawal(withdrawalId, reason) {
    return apiClient.put(`/api/withdrawals/${withdrawalId}/reject`, { reason }).then(r => r.data)
  },
}

/**
 * Notification service for toast messages
 */
export const notificationService = {
  notifications: [],
  listeners: [],

  /**
   * Subscribe to notification changes
   */
  subscribe(callback) {
    this.listeners.push(callback)
    return () => {
      this.listeners = this.listeners.filter((cb) => cb !== callback)
    }
  },

  /**
   * Notify all listeners
   */
  notify() {
    this.listeners.forEach((cb) => cb(this.notifications))
  },

  /**
   * Show success notification
   */
  success(message, duration = 3000) {
    const notification = {
      id: Date.now(),
      type: 'success',
      message,
      duration,
    }
    this.notifications.push(notification)
    this.notify()

    if (duration) {
      setTimeout(() => {
        this.remove(notification.id)
      }, duration)
    }

    return notification.id
  },

  /**
   * Show error notification
   */
  error(message, duration = 5000) {
    const notification = {
      id: Date.now(),
      type: 'error',
      message,
      duration,
    }
    this.notifications.push(notification)
    this.notify()

    if (duration) {
      setTimeout(() => {
        this.remove(notification.id)
      }, duration)
    }

    return notification.id
  },

  /**
   * Show info notification
   */
  info(message, duration = 3000) {
    const notification = {
      id: Date.now(),
      type: 'info',
      message,
      duration,
    }
    this.notifications.push(notification)
    this.notify()

    if (duration) {
      setTimeout(() => {
        this.remove(notification.id)
      }, duration)
    }

    return notification.id
  },

  /**
   * Show warning notification
   */
  warning(message, duration = 4000) {
    const notification = {
      id: Date.now(),
      type: 'warning',
      message,
      duration,
    }
    this.notifications.push(notification)
    this.notify()

    if (duration) {
      setTimeout(() => {
        this.remove(notification.id)
      }, duration)
    }

    return notification.id
  },

  /**
   * Remove notification by ID
   */
  remove(id) {
    this.notifications = this.notifications.filter((n) => n.id !== id)
    this.notify()
  },

  /**
   * Clear all notifications
   */
  clear() {
    this.notifications = []
    this.notify()
  },

  /**
   * Get all notifications
   */
  getAll() {
    return this.notifications
  },
}

export const transactionService = {
  paymentService,
  withdrawalService,
  notificationService,
}

export default {
  paymentService,
  withdrawalService,
  notificationService,
}
