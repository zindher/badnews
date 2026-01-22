import { ref } from 'vue'

/**
 * Google OAuth Service for frontend
 * Handles Google Sign-In integration and token exchange
 */

// Load Google Sign-In script
export const loadGoogleScript = () => {
  return new Promise((resolve, reject) => {
    if (window.google) {
      resolve()
      return
    }

    const script = document.createElement('script')
    script.src = 'https://accounts.google.com/gsi/client'
    script.async = true
    script.defer = true
    script.onload = () => resolve()
    script.onerror = () => reject(new Error('Failed to load Google Sign-In script'))
    document.head.appendChild(script)
  })
}

/**
 * Initialize Google Sign-In button
 * @param {string} clientId - Your Google Cloud Project Client ID
 * @param {string} containerId - ID of container element for the button
 * @param {function} onSuccess - Callback when sign-in succeeds
 * @param {function} onError - Callback when sign-in fails
 */
export const initializeGoogleSignIn = (clientId, containerId, onSuccess, onError) => {
  if (!window.google) {
    console.error('Google Sign-In script not loaded')
    return
  }

  try {
    window.google.accounts.id.initialize({
      client_id: clientId,
      callback: (response) => {
        if (response.credential) {
          onSuccess(response.credential)
        }
      },
    })

    window.google.accounts.id.renderButton(
      document.getElementById(containerId),
      {
        theme: 'outline',
        size: 'large',
        width: '100%',
      }
    )
  } catch (error) {
    console.error('Error initializing Google Sign-In:', error)
    onError(error)
  }
}

/**
 * Sign out user from Google
 */
export const googleSignOut = () => {
  if (window.google) {
    window.google.accounts.id.disableAutoSelect()
  }
}

export const googleAuthService = {
  /**
   * Check if Google Sign-In is available
   */
  isGoogleSignInAvailable() {
    return !!window.google
  },

  /**
   * Get Google Client ID from environment
   */
  getClientId() {
    return import.meta.env.VITE_GOOGLE_CLIENT_ID
  },

  /**
   * Verify that Google token is configured
   */
  validateConfiguration() {
    const clientId = this.getClientId()
    if (!clientId) {
      console.warn('Google Client ID not configured in .env')
      return false
    }
    return true
  },
}
