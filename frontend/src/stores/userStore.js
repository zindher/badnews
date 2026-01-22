import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authService } from '../services/authService'

export const useUserStore = defineStore('user', () => {
  const user = ref(null)
  const token = ref(localStorage.getItem('token'))
  const isLoading = ref(false)
  const error = ref(null)

  // Auto-load user from localStorage on init
  if (token.value) {
    try {
      const storedUser = localStorage.getItem('user')
      if (storedUser) {
        user.value = JSON.parse(storedUser)
      }
    } catch (e) {
      console.error('Error loading user from localStorage:', e)
    }
  }

  // Computed properties
  const isAuthenticated = computed(() => !!token.value && !!user.value)
  const isBuyer = computed(() => user.value?.role === 'Buyer')
  const isMessenger = computed(() => user.value?.role === 'Messenger')
  const isAdmin = computed(() => user.value?.role === 'Admin')
  const userName = computed(() => user.value?.firstName || user.value?.name || 'User')
  const userEmail = computed(() => user.value?.email)

  // Login action
  const login = async (email, password) => {
    isLoading.value = true
    error.value = null
    try {
      const response = await authService.login(email, password)
      setAuthData(response.token, response.user)
      return true
    } catch (err) {
      error.value = err.message || 'Login failed'
      console.error('Login error:', err)
      return false
    } finally {
      isLoading.value = false
    }
  }

  // Register action
  const register = async (userData) => {
    isLoading.value = true
    error.value = null
    try {
      const response = await authService.register(userData)
      setAuthData(response.token, response.user)
      return true
    } catch (err) {
      error.value = err.message || 'Registration failed'
      console.error('Register error:', err)
      return false
    } finally {
      isLoading.value = false
    }
  }

  // Fetch current user profile
  const fetchProfile = async () => {
    isLoading.value = true
    error.value = null
    try {
      const userData = await authService.getProfile()
      user.value = userData
      localStorage.setItem('user', JSON.stringify(userData))
      return userData
    } catch (err) {
      error.value = err.message || 'Failed to fetch profile'
      console.error('Fetch profile error:', err)
      return null
    } finally {
      isLoading.value = false
    }
  }

  // Update user profile
  const updateProfile = async (userData) => {
    isLoading.value = true
    error.value = null
    try {
      const updated = await authService.updateProfile(userData)
      user.value = updated
      localStorage.setItem('user', JSON.stringify(updated))
      return true
    } catch (err) {
      error.value = err.message || 'Failed to update profile'
      console.error('Update profile error:', err)
      return false
    } finally {
      isLoading.value = false
    }
  }

  // Change password
  const changePassword = async (currentPassword, newPassword) => {
    isLoading.value = true
    error.value = null
    try {
      await authService.changePassword(currentPassword, newPassword)
      return true
    } catch (err) {
      error.value = err.message || 'Failed to change password'
      console.error('Change password error:', err)
      return false
    } finally {
      isLoading.value = false
    }
  }

  // Change email
  const changeEmail = async (newEmail) => {
    isLoading.value = true
    error.value = null
    try {
      await authService.changeEmail(newEmail)
      return true
    } catch (err) {
      error.value = err.message || 'Failed to change email'
      console.error('Change email error:', err)
      return false
    } finally {
      isLoading.value = false
    }
  }

  // Logout action
  const logout = async () => {
    try {
      await authService.logout()
    } catch (err) {
      console.error('Logout error:', err)
    } finally {
      user.value = null
      token.value = null
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      error.value = null
    }
  }

  // Set auth data (called after login/register)
  const setAuthData = (newToken, newUser) => {
    token.value = newToken
    user.value = newUser
    localStorage.setItem('token', newToken)
    localStorage.setItem('user', JSON.stringify(newUser))
  }

  return {
    // State
    user,
    token,
    isLoading,
    error,

    // Computed
    isAuthenticated,
    isBuyer,
    isMessenger,
    isAdmin,
    userName,
    userEmail,

    // Actions
    login,
    register,
    fetchProfile,
    updateProfile,
    changePassword,
    changeEmail,
    logout,
    setAuthData
  }
})
