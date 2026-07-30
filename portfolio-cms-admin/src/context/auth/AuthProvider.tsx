import { useState, useEffect, ReactNode } from 'react'
import { AuthContext } from './AuthContext'
import { checkSetup, getMe } from '../../api/auth'

export function AuthProvider({ children }: { children: ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [isLoading, setIsLoading] = useState(true)
  const [requiresSetup, setRequiresSetup] = useState(false)

  useEffect(() => {
    const initialCheck = async () => {
      try {
        const setupResponse = await checkSetup()
        setRequiresSetup(setupResponse.data.requiresSetup)
        await getMe()
        setIsAuthenticated(true)
      } catch {
        setIsAuthenticated(false)
      } finally {
        setIsLoading(false)
      }
    }

    initialCheck()
  }, [])

  return (
    <AuthContext.Provider value={{
      isAuthenticated,
      isLoading,
      requiresSetup,
      setIsAuthenticated
    }}>
      {children}
    </AuthContext.Provider>
  )
}