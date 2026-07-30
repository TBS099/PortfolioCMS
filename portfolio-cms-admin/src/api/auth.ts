import client from './client'
import { LoginDTO, RegisterDTO, ForgotPasswordDTO, ResetPasswordDTO } from '../types'

export const checkSetup = () =>
  client.get('/Auth/setup')

export const register = (data: RegisterDTO) =>
  client.post('/Auth/register', data)

export const login = (data: LoginDTO) =>
  client.post('/Auth/login', data)

export const logout = () =>
  client.post('/Auth/logout')

export const forgotPassword = (data: ForgotPasswordDTO) =>
  client.post('/Auth/forgot-password', data)

export const resetPassword = (data: ResetPasswordDTO) =>
  client.post('/Auth/reset-password', data)

export const getMe = () =>
  client.get('/Auth/me')