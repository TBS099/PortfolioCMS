import client from './client'
import { SocialLinkCreateDTO, SocialLinkUpdateDTO } from '../types'

export const getSocialLinks = () =>
  client.get('/SocialLink')

export const getSocialLinkById = (id: string) =>
  client.get(`/SocialLink/${id}`)

export const createSocialLink = (data: SocialLinkCreateDTO) =>
  client.post('/SocialLink', data)

export const updateSocialLink = (id: string, data: SocialLinkUpdateDTO) =>
  client.put(`/SocialLink/${id}`, data)

export const deleteSocialLink = (id: string) =>
  client.delete(`/SocialLink/${id}`)