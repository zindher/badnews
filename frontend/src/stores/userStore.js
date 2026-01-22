import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useUserStore = defineStore('user', () => {
  const user = ref(null)
  const isAuthenticated = ref(false)
  const token = ref(localStorage.getItem('token'))
  const loading = ref(false)

  const login = async (email, password) => {
    loading.value = true
    try {
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
      })

      if (response.ok) {
        const data = await response.json()
        token.value = data.token
        user.value = data.user
        localStorage.setItem('token', data.token)
        isAuthenticated.value = true
        return true
      }
      return false
    } catch (error) {
      console.error('Login error:', error)
      return false
    } finally {
      loading.value = false
    }
  }

  const register = async (email, password, name, userType) => {
    loading.value = true
    try {
      const response = await fetch('/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password, name, userType })
      })

      if (response.ok) {
        const data = await response.json()
        token.value = data.token
        user.value = data.user
        localStorage.setItem('token', data.token)
        isAuthenticated.value = true
        return true
      }
      return false
    } catch (error) {
      console.error('Register error:', error)
      return false
    } finally {
      loading.value = false
    }
  }

  const logout = () => {
    user.value = null
    isAuthenticated.value = false
    token.value = null
    localStorage.removeItem('token')
  }

  const refreshUser = async () => {
    if (!token.value) return false

    try {
      const response = await fetch('/api/auth/me', {
        headers: { 'Authorization': `Bearer ${token.value}` }
      })

      if (response.ok) {
        user.value = await response.json()
        return true
      }
      return false
    } catch (error) {
      console.error('Refresh user error:', error)
      return false
    }
  }

  return { user, isAuthenticated, token, loading, login, register, logout, refreshUser }
})
