import axios from 'axios'

const client = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:7174/api',
  withCredentials: true, // sends the httpOnly cookie with every request
})

// Interceptor to handle 401 Unauthorized responses
client.interceptors.response.use(
  (response) => response,
  (error) => {
    const isAuthEndpoint = error.config?.url?.startsWith('/Auth')
    const alreadyOnLogin = window.location.pathname === '/login'

    if (error.response?.status === 401 && !isAuthEndpoint && !alreadyOnLogin) {
      window.location.href = '/login'
    }

    return Promise.reject(error)
  },
)

export default client