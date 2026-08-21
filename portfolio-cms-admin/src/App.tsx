import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./context/auth/AuthProvider";
import { useAuth } from "./hooks/useAuth";
import AppLayout from "./components/layout/AppLayout";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import ForgotPassword from "./pages/auth/ForgotPassword";
import ResetPassword from "./pages/auth/ResetPassword";
import Hero from "./pages/hero/Hero";
import { Toaster } from "@/components/ui/sonner";
import { useTheme } from "@/hooks/useTheme";
import About from "./pages/about/About";
import Experience from "./pages/experience/Experience";
import Projects from "./pages/projects/Projects";
import SocialLinks from "./pages/social-links/SocialLinks";
import Dashboard from "./pages/dashboard/Dashboard";
import Files from "./pages/files/Files";
import NotFound from "./pages/not-found/NotFound";
import { LoadingState } from "./components/ui/loading-state";

function AppRoutes() {
  const { isAuthenticated, isLoading, requiresSetup } = useAuth();

  if (isLoading) return <LoadingState></LoadingState>;

  if (requiresSetup)
    return (
      <Routes>
        <Route path="*" element={<Register />} />
      </Routes>
    );

  if (!isAuthenticated)
    return (
      <Routes>
        <Route path="/forgot-password" element={<ForgotPassword />} />
        <Route path="/reset-password" element={<ResetPassword />} />
        <Route path="*" element={<Login />} />
      </Routes>
    );

  return (
    <AppLayout>
      <Routes>
        <Route path="/" element={<Dashboard />} />
        <Route path="/hero" element={<Hero />} />
        <Route path="/about" element={<About />} />
        <Route path="/experience" element={<Experience />} />
        <Route path="/projects" element={<Projects />} />
        <Route path="/social-links" element={<SocialLinks />} />
        <Route path="/files" element={<Files />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </AppLayout>
  );
}

export default function App() {
  const { theme } = useTheme();

  return (
    <BrowserRouter>
      <AuthProvider>
        <Toaster richColors position="top-right" theme={theme} />
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  );
}
