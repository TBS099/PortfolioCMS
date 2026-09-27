import { useEffect, useState } from "react";
import { ThemeContext, type Theme } from "./ThemeContext";

export function ThemeProvider({ children }: { children: React.ReactNode }) {
  const [theme, setTheme] = useState<Theme>(
    () => (localStorage.getItem("theme") as Theme) || "dark",
  );

  useEffect(() => {
    const root = window.document.documentElement;
    root.classList.remove("light", "dark");

    const setFavicon = (resolved: "light" | "dark") => {
      const favicon = document.getElementById(
        "favicon",
      ) as HTMLLinkElement | null;
      if (favicon)
        favicon.href =
          resolved === "dark"
            ? "/src/assets/images/favicon-dark.svg"
            : "/src/assets/images/favicon-light.svg";
    };

    if (theme === "system") {
      const systemTheme = window.matchMedia("(prefers-color-scheme: dark)")
        .matches
        ? "dark"
        : "light";
      root.classList.add(systemTheme);
      setFavicon(systemTheme);
      return;
    }

    root.classList.add(theme);
    setFavicon(theme);
    localStorage.setItem("theme", theme);
  }, [theme]);

  return (
    <ThemeContext.Provider value={{ theme, setTheme }}>
      {children}
    </ThemeContext.Provider>
  );
}
