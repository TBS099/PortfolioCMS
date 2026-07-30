import axios from 'axios'

const client = axios.create({
  baseURL: 'https://localhost:7174/api',
  withCredentials: true, // sends the httpOnly cookie with every request
})

export default client