import { createContext } from 'react'

interface AuthContextType {
  isAuthenticated: boolean
  isLoading: boolean
  requiresSetup: boolean
  setIsAuthenticated: (value: boolean) => void
}

export const AuthContext = createContext<AuthContextType | null>(null)