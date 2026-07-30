import client from './client'

export const getFiles = () =>
  client.get('/FileUpload')

export const getFilesByCategory = (category: string) =>
  client.get(`/FileUpload/${category}`)

export const uploadFile = (file: File, category: string) => {
  const formData = new FormData()
  formData.append('file', file)
  formData.append('category', category)
  return client.post('/FileUpload', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
}

export const deleteFile = (id: string) =>
  client.delete(`/FileUpload/${id}`)