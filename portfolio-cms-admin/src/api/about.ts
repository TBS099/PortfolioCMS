import client from './client'
import { AboutUpdateDTO } from '../types'

export const getAbout = () =>
  client.get('/About')

export const updateAbout = (data: AboutUpdateDTO) =>
  client.put('/About', data)