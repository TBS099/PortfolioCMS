import { useState, useEffect } from "react";
import { getHero, updateHero } from "@/api/hero";
import { HeroDTO, HeroUpdateDTO } from "@/types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { toast } from "sonner";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card";

export default function Hero() {
  const [formData, setFormData] = useState<HeroUpdateDTO>({
    name: "",
    title: "",
    subtitle: "",
    imageUrl: "",
  });
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);

  // Fetch existing hero data when the page loads
  useEffect(() => {
    const fetchHero = async () => {
      try {
        const response = await getHero();
        const hero: HeroDTO = response.data;

        // Pre-fill the form with existing data
        setFormData({
          name: hero.name ?? "",
          title: hero.title,
          subtitle: hero.subtitle ?? "",
          imageUrl: hero.imageUrl ?? "",
        });
      } catch {
        // Hero doesn't exist yet - form stays empty, that's fine
      } finally {
        setIsLoading(false);
      }
    };

    fetchHero();
  }, []);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsSaving(true);

    try {
      await updateHero(formData);
      toast.success("Hero updated successfully.");
    } catch {
      toast.error("Failed to save. Please try again.");
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-48">
        <p className="text-muted-foreground">Loading...</p>
      </div>
    );
  }

  return (
    <div className="max-w-3xl mx-auto">
      <div className="mb-6 mt-8">
        <h1 className="text-2xl font-bold">Hero Section</h1>
        <p className="text-muted-foreground mt-1">
          This is the first thing visitors see on your portfolio.
        </p>
      </div>

      <Card>
        <div className="w-full max-w-xl mx-auto">
          <CardHeader>
            <CardTitle>Edit Hero</CardTitle>
            <CardDescription className="pb-10">
              Changes are saved immediately and reflected on your portfolio.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="name">Name</Label>
                <Input
                  id="name"
                  type="text"
                  placeholder="John Doe"
                  value={formData.name ?? ""}
                  onChange={(e) =>
                    setFormData({ ...formData, name: e.target.value })
                  }
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="title">
                  Title <span className="text-destructive">*</span>
                </Label>
                <Input
                  id="title"
                  type="text"
                  placeholder="Full Stack Developer"
                  value={formData.title}
                  onChange={(e) =>
                    setFormData({ ...formData, title: e.target.value })
                  }
                  required
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="subtitle">Subtitle</Label>
                <Input
                  id="subtitle"
                  type="text"
                  placeholder="Building things for the web"
                  value={formData.subtitle ?? ""}
                  onChange={(e) =>
                    setFormData({ ...formData, subtitle: e.target.value })
                  }
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="imageUrl">Image URL</Label>
                <Input
                  id="imageUrl"
                  type="url"
                  placeholder="https://example.com/photo.jpg"
                  value={formData.imageUrl ?? ""}
                  onChange={(e) =>
                    setFormData({ ...formData, imageUrl: e.target.value })
                  }
                />
              </div>

              <Button
                type="submit"
                disabled={isSaving}
                className="w-full mt-4 mb-6"
              >
                {isSaving ? "Saving..." : "Save Changes"}
              </Button>
            </form>
          </CardContent>
        </div>
      </Card>
    </div>
  );
}
