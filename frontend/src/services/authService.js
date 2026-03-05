import apiClient from './apiClient'

export const authService = {
  /**
   * Register new user
   * @param {Object} userData - { email, password, firstName, lastName, role, termsAcceptedAt }
   * @returns {Promise<{token, user}>}
   */
  async register(userData) {
    const response = await apiClient.post('/api/auth/register', userData)
    const { data } = response.data
    return {
      token: data.token,
      user: {
        id: data.userId,
        email: data.email,
        firstName: data.firstName,
        lastName: data.lastName,
        role: data.role,
      },
    }
  },

  /**
   * Login user
   * @param {string} email
   * @param {string} password
   * @returns {Promise<{token, user}>}
   */
  async login(email, password) {
    const response = await apiClient.post('/api/auth/login', { email, password })
    const { data } = response.data
    return {
      token: data.token,
      user: {
        id: data.userId,
        email: data.email,
        firstName: data.firstName,
        lastName: data.lastName,
        role: data.role,
      },
    }
  },

  /**
   * Get current user profile
   * @returns {Promise<Object>}
   */
  async getProfile() {
    const response = await apiClient.get('/api/users/me')
    return response.data.data
  },

  /**
   * Update user profile
   * @param {Object} userData
   * @returns {Promise<Object>}
   */
  async updateProfile(userData) {
    const response = await apiClient.put('/api/users/me', userData)
    return response.data.data
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
   * @returns {Promise<{token, user}>}
   */
  async loginWithGoogle(googleToken, role = null) {
    const payload = { googleToken }
    if (role) payload.role = role
    const response = await apiClient.post('/api/auth/google-login', payload)
    const { data } = response.data
    return {
      token: data.token,
      user: {
        id: data.userId,
        email: data.email,
        firstName: data.firstName,
        lastName: data.lastName,
        role: data.role,
        profilePicture: data.profilePicture,
      },
    }
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
