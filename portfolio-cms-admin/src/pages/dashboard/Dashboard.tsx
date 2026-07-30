import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getDashboard } from "@/api/dashboard";
import { DashboardDTO } from "@/types";
import { toast } from "sonner";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";

const formatLastUpdated = (dateString: string) => {
  const date = new Date(dateString);
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));

  if (diffDays === 0) return "Updated today";
  if (diffDays === 1) return "Updated yesterday";
  if (diffDays < 7) return `Updated ${diffDays} days ago`;
  if (diffDays < 30)
    return `Updated ${Math.floor(diffDays / 7)} week${Math.floor(diffDays / 7) > 1 ? "s" : ""} ago`;
  if (diffDays < 365)
    return `Updated ${Math.floor(diffDays / 30)} month${Math.floor(diffDays / 30) > 1 ? "s" : ""} ago`;
  return `Updated ${Math.floor(diffDays / 365)} year${Math.floor(diffDays / 365) > 1 ? "s" : ""} ago`;
};

const isStale = (dateString: string) => {
  const diffMs = new Date().getTime() - new Date(dateString).getTime();
  return Math.floor(diffMs / (1000 * 60 * 60 * 24)) > 90;
};

export default function Dashboard() {
  const navigate = useNavigate();
  const [data, setData] = useState<DashboardDTO | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;

    const fetchDashboard = async () => {
      try {
        const response = await getDashboard();
        if (!cancelled) setData(response.data);
      } catch {
        if (!cancelled) toast.error("Failed to load dashboard.");
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    };

    fetchDashboard();
    return () => {
      cancelled = true;
    };
  }, []);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-48">
        <p className="text-muted-foreground">Loading...</p>
      </div>
    );
  }

  if (!data) return null;

  const sections = [
    {
      key: "hero",
      label: "Hero",
      path: "/hero",
      isConfigured: data.hero.isConfigured,
      lastUpdated: data.hero.updatedAt,
      description: data.hero.isConfigured
        ? "Your hero section is set up."
        : "Your hero section is not configured yet.",
    },
    {
      key: "about",
      label: "About",
      path: "/about",
      isConfigured: data.about.isConfigured,
      lastUpdated: data.about.updatedAt,
      description: data.about.isConfigured
        ? "Your about section is set up."
        : "Your about section is not configured yet.",
    },
    {
      key: "experience",
      label: "Experience",
      path: "/experience",
      isConfigured: data.experience.count > 0,
      lastUpdated: data.experience.lastUpdatedAt,
      description:
        data.experience.count > 0
          ? `${data.experience.count} entr${data.experience.count === 1 ? "y" : "ies"}`
          : "No experience entries yet.",
    },
    {
      key: "projects",
      label: "Projects",
      path: "/projects",
      isConfigured: data.projects.count > 0,
      lastUpdated: data.projects.lastUpdatedAt,
      description:
        data.projects.count > 0
          ? `${data.projects.count} project${data.projects.count === 1 ? "" : "s"}`
          : "No projects yet.",
    },
    {
      key: "socialLinks",
      label: "Social Links",
      path: "/social-links",
      isConfigured: data.socialLinks.count > 0,
      lastUpdated: data.socialLinks.lastUpdatedAt,
      description:
        data.socialLinks.count > 0
          ? `${data.socialLinks.count} link${data.socialLinks.count === 1 ? "" : "s"}`
          : "No social links yet.",
    },
  ];

  const allConfigured = sections.every((s) => s.isConfigured);

  return (
    <div className="max-w-4xl mx-auto">
      <div className="mb-6 mt-8">
        <h1 className="text-2xl font-bold">Dashboard</h1>
        <p className="text-muted-foreground mt-1">
          {allConfigured
            ? "Your portfolio is fully set up."
            : "Complete the sections below to finish setting up your portfolio."}
        </p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {sections.map((section) => (
          <Card
            key={section.key}
            className={`transition-colors ${
              !section.isConfigured
                ? "border-destructive/50"
                : section.lastUpdated && isStale(section.lastUpdated)
                  ? "border-yellow-500/50"
                  : "border-border"
            }`}
          >
            <CardHeader className="pb-2">
              <div className="flex items-center justify-between">
                <CardTitle className="text-base">{section.label}</CardTitle>
                <span
                  className={`text-xs px-2 py-0.5 rounded-full ${
                    section.isConfigured
                      ? "bg-green-500/10 text-green-500"
                      : "bg-destructive/10 text-destructive"
                  }`}
                >
                  {section.isConfigured ? "Active" : "Empty"}
                </span>
              </div>
            </CardHeader>
            <CardContent className="flex flex-col justify-between h-24">
              <div>
                <p className="text-sm text-muted-foreground">
                  {section.description}
                </p>
                {section.lastUpdated && (
                  <p
                    className={`text-xs mt-1 ${
                      isStale(section.lastUpdated)
                        ? "text-yellow-500"
                        : "text-muted-foreground"
                    }`}
                  >
                    {formatLastUpdated(section.lastUpdated)}
                    {isStale(section.lastUpdated) && " — consider a refresh"}
                  </p>
                )}
              </div>
              <Button
                variant="outline"
                size="sm"
                className="w-full mt-3"
                onClick={() => navigate(section.path)}
              >
                {section.isConfigured ? "Manage" : "Set up"}
              </Button>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}
