import { useState, useEffect, useRef, ReactNode } from "react";
import { useLocation } from "react-router-dom";
import { AuthContext } from "./AuthContext";
import { checkSetup, getMe } from "../../api/auth";

export function AuthProvider({ children }: { children: ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [requiresSetup, setRequiresSetup] = useState(false);
  const location = useLocation();
  const hasMounted = useRef(false);

  useEffect(() => {
    const initialCheck = async () => {
      try {
        const setupResponse = await checkSetup();
        setRequiresSetup(setupResponse.data.requiresSetup);
        await getMe();
        setIsAuthenticated(true);
      } catch {
        setIsAuthenticated(false);
      } finally {
        setIsLoading(false);
      }
    };

    initialCheck();
  }, []);

  // This effect runs whenever the location changes or the authentication state changes
  useEffect(() => {
    if (!hasMounted.current) {
      // Skip the run that fires on initial mount - the effect above
      // already just did this exact check.
      hasMounted.current = true;
      return;
    }

    if (!isAuthenticated) return;

    getMe().catch(() => setIsAuthenticated(false));
  }, [location.pathname, isAuthenticated]);

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        isLoading,
        requiresSetup,
        setIsAuthenticated,
        setRequiresSetup,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
