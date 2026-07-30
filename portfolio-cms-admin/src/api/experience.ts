import client from './client'
import { ExperienceCreateDTO, ExperienceUpdateDTO } from '../types'

export const getExperiences = () =>
  client.get('/Experience')

export const getExperienceById = (id: string) =>
  client.get(`/Experience/${id}`)

export const createExperience = (data: ExperienceCreateDTO) =>
  client.post('/Experience', data)

export const updateExperience = (id: string, data: ExperienceUpdateDTO) =>
  client.put(`/Experience/${id}`, data)

export const deleteExperience = (id: string) =>
  client.delete(`/Experience/${id}`)