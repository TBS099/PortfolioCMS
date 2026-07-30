import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import { useTheme } from "@/hooks/useTheme";
import { logout } from "@/api/auth";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuItem,
  SidebarHeader,
} from "@/components/ui/sidebar";

const navItems = [
  { label: "Dashboard", path: "/" },
  { label: "Hero", path: "/hero" },
  { label: "About", path: "/about" },
  { label: "Experience", path: "/experience" },
  { label: "Projects", path: "/projects" },
  { label: "Social Links", path: "/social-links" },
];

export function AppSidebar() {
  const navigate = useNavigate();
  const location = useLocation();
  const { setIsAuthenticated } = useAuth();
  const { theme, setTheme } = useTheme();

  const handleLogout = async () => {
    try {
      await logout();
    } finally {
      setIsAuthenticated(false);
      navigate("/");
    }
  };

  const toggleTheme = () => {
    setTheme(theme === "dark" ? "light" : "dark");
  };

  return (
    <Sidebar>
      <SidebarHeader className="px-4 py-4 border-b border-sidebar-border">
        <h2 className="text-lg font-semibold">Portfolio CMS</h2>
      </SidebarHeader>

      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel className="px-4 py-2 text-xs uppercase tracking-wider text-sidebar-foreground/50">
            Navigation
          </SidebarGroupLabel>
          <SidebarMenu>
            {navItems.map((item, index) => (
              <SidebarMenuItem key={item.path}>
                <button
                  onClick={() => navigate(item.path)}
                  className={`
                    w-full text-left px-4 py-2.5 text-sm
                    ${index !== 0 ? "border-t border-sidebar-border" : ""}
                    ${
                      location.pathname === item.path
                        ? "text-sidebar-foreground font-medium"
                        : "text-sidebar-foreground/70 hover:bg-sidebar-accent hover:text-sidebar-accent-foreground"
                    }
                    transition-colors
                  `}
                >
                  {item.label}
                </button>
              </SidebarMenuItem>
            ))}
          </SidebarMenu>
        </SidebarGroup>
      </SidebarContent>

      <SidebarFooter className="border-t border-sidebar-border">
        <button
          onClick={toggleTheme}
          className="w-full text-sm text-left px-4 py-3 border-b border-sidebar-border text-sidebar-foreground/70 hover:bg-sidebar-accent hover:text-sidebar-accent-foreground transition-colors"
        >
          {theme === "dark" ? "Switch to Light" : "Switch to Dark"}
        </button>
        <button
          onClick={handleLogout}
          className="w-full text-sm text-left px-4 py-3 text-sidebar-foreground/70 hover:bg-destructive hover:text-destructive-foreground transition-colors"
        >
          Logout
        </button>
      </SidebarFooter>
    </Sidebar>
  );
}
