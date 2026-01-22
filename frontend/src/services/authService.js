import apiClient from './apiClient'

export const authService = {
  /**
   * Register new user
   * @param {Object} userData - { email, password, firstName, lastName, role }
   * @returns {Promise}
   */
  register(userData) {
    return apiClient.post('/api/auth/register', userData)
  },

  /**
   * Login user
   * @param {string} email
   * @param {string} password
   * @returns {Promise}
   */
  login(email, password) {
    return apiClient.post('/api/auth/login', { email, password })
  },

  /**
   * Get current user profile
   * @returns {Promise}
   */
  getProfile() {
    return apiClient.get('/api/users/me')
  },

  /**
   * Update user profile
   * @param {Object} userData
   * @returns {Promise}
   */
  updateProfile(userData) {
    return apiClient.put('/api/users/me', userData)
  },

  /**
   * Change password
   * @param {string} currentPassword
   * @param {string} newPassword
   * @returns {Promise}
   */
  changePassword(currentPassword, newPassword) {
    return apiClient.put('/api/users/me/password', {
      currentPassword,
      newPassword,
    })
  },

  /**
   * Change email
   * @param {string} newEmail
   * @returns {Promise}
   */
  changeEmail(newEmail) {
    return apiClient.put('/api/users/me/email', {
      newEmail,
    })
  },

  /**
   * Verify email with token
   * @param {string} token
   * @returns {Promise}
   */
  verifyEmail(token) {
    return apiClient.post('/api/auth/verify-email', { token })
  },

  /**
   * Request password reset
   * @param {string} email
   * @returns {Promise}
   */
  requestPasswordReset(email) {
    return apiClient.post('/api/auth/request-password-reset', { email })
  },

  /**
   * Reset password with token
   * @param {string} token
   * @param {string} newPassword
   * @returns {Promise}
   */
  resetPassword(token, newPassword) {
    return apiClient.post('/api/auth/reset-password', { token, newPassword })
  },

  /**
   * Logout (clear local storage)
   */
  logout() {
    localStorage.removeItem('auth_token')
    localStorage.removeItem('user')
  },

  /**
   * Login with Google OAuth token
   * @param {string} googleToken - Token from Google Sign-In
   * @param {string} role - Optional: 'Buyer' or 'Messenger'
   * @returns {Promise}
   */
  loginWithGoogle(googleToken, role = null) {
    const payload = { googleToken }
    if (role) payload.role = role
    return apiClient.post('/api/auth/google-login', payload)
  },

  /**
   * Link Google account to existing user
   * @param {string} googleToken - Token from Google Sign-In
   * @returns {Promise}
   */
  linkGoogleAccount(googleToken) {
    return apiClient.post('/api/auth/link-google', { googleToken })
  },

  /**
   * Unlink Google account from user
   * @returns {Promise}
   */
  unlinkGoogleAccount() {
    return apiClient.post('/api/auth/unlink-google', {})
  },

  /**
   * Check if user is authenticated
   * @returns {boolean}
   */
  isAuthenticated() {
    return !!localStorage.getItem('auth_token')
  },

  /**
   * Get stored token
   * @returns {string|null}
   */
  getToken() {
    return localStorage.getItem('auth_token')
  },

  /**
   * Get stored user data
   * @returns {Object|null}
   */
  getUser() {
    const user = localStorage.getItem('user')
    return user ? JSON.parse(user) : null
  },

  /**
   * Store auth data
   * @param {string} token
   * @param {Object} user
   */
  setAuthData(token, user) {
    localStorage.setItem('auth_token', token)
    localStorage.setItem('user', JSON.stringify(user))
  },
}

export default authService
