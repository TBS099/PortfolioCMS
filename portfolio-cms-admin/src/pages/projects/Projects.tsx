import { useState, useEffect } from "react";
import {
  getProjects,
  createProject,
  updateProject,
  deleteProject,
} from "@/api/project";
import { ProjectDTO, ProjectCreateDTO } from "@/types";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { LoadingState } from "@/components/ui/loading-state";

const emptyForm: ProjectCreateDTO = {
  title: "",
  slug: "",
  description: "",
  imageUrl: "",
  liveUrl: "",
  githubUrl: "",
  stack: "",
  isFeatured: false,
};

export default function Projects() {
  const [projects, setProjects] = useState<ProjectDTO[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState<ProjectCreateDTO>(emptyForm);

  useEffect(() => {
    let cancelled = false;

    const fetchProjects = async () => {
      try {
        const response = await getProjects();
        if (!cancelled) setProjects(response.data);
      } catch {
        if (!cancelled) toast.error("Failed to load projects.");
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    };

    fetchProjects();

    return () => {
      cancelled = true;
    };
  }, []);

  const openCreateModal = () => {
    setEditingId(null);
    setFormData(emptyForm);
    setIsModalOpen(true);
  };

  const openEditModal = (project: ProjectDTO) => {
    setEditingId(project.id);
    setFormData({
      title: project.title,
      slug: project.slug,
      description: project.description,
      imageUrl: project.imageUrl ?? "",
      liveUrl: project.liveUrl ?? "",
      githubUrl: project.githubUrl ?? "",
      stack: project.stack,
      isFeatured: project.isFeatured,
    });
    setIsModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this project?")) return;

    try {
      await deleteProject(id);
      setProjects((prev) => prev.filter((p) => p.id !== id));
      toast.success("Project deleted.");
    } catch {
      toast.error("Failed to delete project.");
    }
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsSaving(true);

    const payload = {
      ...formData,
      imageUrl: formData.imageUrl || undefined,
      liveUrl: formData.liveUrl || undefined,
      githubUrl: formData.githubUrl || undefined,
    };

    try {
      if (editingId) {
        const response = await updateProject(editingId, payload);
        setProjects((prev) =>
          prev.map((p) => (p.id === editingId ? response.data : p)),
        );
        toast.success("Project updated.");
      } else {
        const response = await createProject(payload);
        setProjects((prev) => [...prev, response.data]);
        toast.success("Project created.");
      }
      setIsModalOpen(false);
    } catch {
      toast.error("Failed to save project.");
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return <LoadingState />;
  }

  return (
    <div className="max-w-4xl mx-auto">
      <div className="mb-6 mt-8 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Projects</h1>
          <p className="text-muted-foreground mt-1">
            Manage your portfolio projects.
          </p>
        </div>
        <Button onClick={openCreateModal}>Add Project</Button>
      </div>

      {projects.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">
          No projects yet. Click "Add Project" to get started.
        </div>
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Title</TableHead>
              <TableHead>Stack</TableHead>
              <TableHead>Featured</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {projects.map((project) => (
              <TableRow key={project.id}>
                <TableCell className="font-medium">{project.title}</TableCell>
                <TableCell className="text-muted-foreground text-sm">
                  {project.stack}
                </TableCell>
                <TableCell>
                  {project.isFeatured ? (
                    <span className="text-xs bg-primary text-primary-foreground px-2 py-0.5 rounded-full">
                      Featured
                    </span>
                  ) : (
                    <span className="text-muted-foreground text-xs">—</span>
                  )}
                </TableCell>
                <TableCell className="text-right space-x-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => openEditModal(project)}
                  >
                    Edit
                  </Button>
                  <Button
                    variant="destructive"
                    size="sm"
                    onClick={() => handleDelete(project.id)}
                  >
                    Delete
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}

      <Dialog open={isModalOpen} onOpenChange={setIsModalOpen}>
        <DialogContent className="max-w-lg bg-card max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>
              {editingId ? "Edit Project" : "Add Project"}
            </DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="title">
                Title <span className="text-destructive">*</span>
              </Label>
              <Input
                id="title"
                placeholder="My Awesome Project"
                value={formData.title}
                onChange={(e) =>
                  setFormData({ ...formData, title: e.target.value })
                }
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="slug">
                Slug <span className="text-destructive">*</span>
              </Label>
              <Input
                id="slug"
                placeholder="my-awesome-project"
                value={formData.slug}
                onChange={(e) =>
                  setFormData({ ...formData, slug: e.target.value })
                }
                required
              />
              <p className="text-xs text-muted-foreground">
                Lowercase letters, numbers and hyphens only. e.g.
                my-project-2024
              </p>
            </div>

            <div className="space-y-2">
              <Label htmlFor="description">
                Description <span className="text-destructive">*</span>
              </Label>
              <Textarea
                id="description"
                placeholder="What does this project do?"
                value={formData.description}
                onChange={(e) =>
                  setFormData({ ...formData, description: e.target.value })
                }
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="stack">
                Stack <span className="text-destructive">*</span>
              </Label>
              <Input
                id="stack"
                placeholder="React, TypeScript, .NET, SQL Server"
                value={formData.stack}
                onChange={(e) =>
                  setFormData({ ...formData, stack: e.target.value })
                }
                required
              />
              <p className="text-xs text-muted-foreground">
                Comma separated list of technologies.
              </p>
            </div>

            <div className="space-y-2">
              <Label htmlFor="imageUrl">Image URL</Label>
              <Input
                id="imageUrl"
                type="url"
                placeholder="https://example.com/screenshot.jpg"
                value={formData.imageUrl ?? ""}
                onChange={(e) =>
                  setFormData({ ...formData, imageUrl: e.target.value })
                }
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="liveUrl">Live URL</Label>
              <Input
                id="liveUrl"
                type="url"
                placeholder="https://myproject.com"
                value={formData.liveUrl ?? ""}
                onChange={(e) =>
                  setFormData({ ...formData, liveUrl: e.target.value })
                }
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="githubUrl">GitHub URL</Label>
              <Input
                id="githubUrl"
                type="url"
                placeholder="https://github.com/username/repo"
                value={formData.githubUrl ?? ""}
                onChange={(e) =>
                  setFormData({ ...formData, githubUrl: e.target.value })
                }
              />
            </div>

            <div className="flex items-center gap-2">
              <Checkbox
                id="isFeatured"
                checked={formData.isFeatured}
                onCheckedChange={(checked) =>
                  setFormData({ ...formData, isFeatured: checked === true })
                }
              />
              <Label htmlFor="isFeatured" className="cursor-pointer">
                Featured project
              </Label>
            </div>

            <div className="flex justify-end gap-2 pt-2">
              <Button
                type="button"
                variant="outline"
                onClick={() => setIsModalOpen(false)}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={isSaving}>
                {isSaving
                  ? "Saving..."
                  : editingId
                    ? "Save Changes"
                    : "Add Project"}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
}
