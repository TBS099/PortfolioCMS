import client from './client'

export const getFiles = () =>
  client.get('/FileUpload')

export const getPublicFiles = () =>
  client.get('/FileUpload/public')

export const uploadFile = (
  file: File,
  category: string,
  displayName?: string,
  isPublic: boolean = true
) => {
  const formData = new FormData()
  formData.append('file', file)
  formData.append('category', category)
  if (displayName) formData.append('displayName', displayName)
  formData.append('isPublic', String(isPublic))
  return client.post('/FileUpload', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
}

export const updateVisibility = (id: string, isPublic: boolean) =>
  client.patch(`/FileUpload/${id}/visibility`, isPublic, {
    headers: { 'Content-Type': 'application/json' }
  })

export const deleteFile = (id: string) =>
  client.delete(`/FileUpload/${id}`)