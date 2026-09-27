import axios from 'axios'

const client = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:7174/api',
  withCredentials: true, // sends the httpOnly cookie with every request
})

export default client