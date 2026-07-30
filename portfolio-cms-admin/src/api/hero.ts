import client from './client'
import { HeroUpdateDTO } from '../types'

export const getHero = () =>
  client.get('/Hero')

export const updateHero = (data: HeroUpdateDTO) =>
  client.put('/Hero', data)