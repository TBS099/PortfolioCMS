import client from './client'
import { ProjectCreateDTO, ProjectUpdateDTO } from '../types'

export const getProjects = () =>
  client.get('/Project')

export const getProjectById = (id: string) =>
  client.get(`/Project/${id}`)

export const createProject = (data: ProjectCreateDTO) =>
  client.post('/Project', data)

export const updateProject = (id: string, data: ProjectUpdateDTO) =>
  client.put(`/Project/${id}`, data)

export const deleteProject = (id: string) =>
  client.delete(`/Project/${id}`)