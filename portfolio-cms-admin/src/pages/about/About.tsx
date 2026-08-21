import { useState, useEffect } from "react";
import { getAbout, updateAbout } from "@/api/about";
import { AboutDTO, AboutUpdateDTO } from "@/types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { toast } from "sonner";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card";
import { LoadingState } from "@/components/ui/loading-state";

export default function About() {
  const [formData, setFormData] = useState<AboutUpdateDTO>({
    title: "",
    body: "",
    imageUrl: "",
    tagline: "",
  });
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);

  // Fetch existing About data when the page loads
  useEffect(() => {
    const fetchAbout = async () => {
      try {
        const response = await getAbout();
        const About: AboutDTO = response.data;

        // Pre-fill the form with existing data
        setFormData({
          title: About.title,
          body: About.body ?? "",
          imageUrl: About.imageUrl ?? "",
          tagline: About.tagline ?? "",
        });
      } catch {
        // About doesn't exist yet - form stays empty, that's fine
      } finally {
        setIsLoading(false);
      }
    };

    fetchAbout();
  }, []);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsSaving(true);

    try {
      await updateAbout(formData);
      toast.success("About updated successfully.");
    } catch {
      toast.error("Failed to save. Please try again.");
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return <LoadingState />;
  }

  return (
    <div className="max-w-3xl mx-auto">
      <div className="mb-6 mt-8">
        <h1 className="text-2xl font-bold">About Section</h1>
        <p className="text-muted-foreground mt-1">
          A description of yourself to potential employers or clients. Try to
          make yourself look good.
        </p>
      </div>

      <Card>
        <div className="w-full max-w-xl mx-auto">
          <CardHeader>
            <CardTitle>Edit About</CardTitle>
            <CardDescription className="pb-10">
              Changes are saved immediately and reflected on your portfolio.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="title">
                  Title <span className="text-destructive">*</span>
                </Label>
                <Input
                  id="title"
                  type="text"
                  placeholder="Software Engineer"
                  value={formData.title ?? ""}
                  onChange={(e) =>
                    setFormData({ ...formData, title: e.target.value })
                  }
                  required
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="body">
                  Body <span className="text-destructive">*</span>
                </Label>
                <Textarea
                  id="body"
                  placeholder="Tell us about yourself..."
                  value={formData.body ?? ""}
                  onChange={(e) =>
                    setFormData({ ...formData, body: e.target.value })
                  }
                  required
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

              <div className="space-y-2">
                <Label htmlFor="tagline">Tagline</Label>
                <Input
                  id="tagline"
                  type="text"
                  placeholder="e.g. Building things for the web"
                  value={formData.tagline ?? ""}
                  onChange={(e) =>
                    setFormData({ ...formData, tagline: e.target.value })
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
