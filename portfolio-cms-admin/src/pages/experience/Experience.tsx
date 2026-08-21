import { useState, useEffect } from "react";
import {
  getExperiences,
  createExperience,
  updateExperience,
  deleteExperience,
} from "@/api/experience";
import { ExperienceDTO, ExperienceCreateDTO } from "@/types";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
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

const emptyForm: ExperienceCreateDTO = {
  title: "",
  organization: "",
  type: "WORK",
  startDate: "",
  endDate: "",
  location: "",
  description: "",
};

export default function Experience() {
  const [experiences, setExperiences] = useState<ExperienceDTO[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState<ExperienceCreateDTO>(emptyForm);

  useEffect(() => {
    let cancelled = false;

    const fetchExperiences = async () => {
      try {
        const response = await getExperiences();
        if (!cancelled) setExperiences(response.data);
      } catch {
        if (!cancelled) toast.error("Failed to load experiences.");
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    };

    fetchExperiences();

    return () => {
      cancelled = true;
    };
  }, []);

  const openCreateModal = () => {
    setEditingId(null);
    setFormData(emptyForm);
    setIsModalOpen(true);
  };

  const openEditModal = (experience: ExperienceDTO) => {
    setEditingId(experience.id);
    setFormData({
      title: experience.title,
      organization: experience.organization,
      type: experience.type,
      startDate: experience.startDate.slice(0, 7), // format to YYYY-MM for month input
      endDate: experience.endDate ? experience.endDate.slice(0, 7) : "",
      location: experience.location ?? "",
      description: experience.description ?? "",
    });
    setIsModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this experience?")) return;

    try {
      await deleteExperience(id);
      setExperiences((prev) => prev.filter((e) => e.id !== id));
      toast.success("Experience deleted.");
    } catch {
      toast.error("Failed to delete experience.");
    }
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsSaving(true);

    // Convert YYYY-MM to a full ISO date string the backend expects
    const payload = {
      ...formData,
      startDate: formData.startDate ? `${formData.startDate}-01` : "",
      endDate: formData.endDate ? `${formData.endDate}-01` : undefined,
      location: formData.location || undefined,
      description: formData.description || undefined,
    };

    try {
      if (editingId) {
        const response = await updateExperience(editingId, payload);
        setExperiences((prev) =>
          prev.map((e) => (e.id === editingId ? response.data : e)),
        );
        toast.success("Experience updated.");
      } else {
        const response = await createExperience(payload);
        setExperiences((prev) => [...prev, response.data]);
        toast.success("Experience created.");
      }
      setIsModalOpen(false);
    } catch {
      toast.error("Failed to save experience.");
    } finally {
      setIsSaving(false);
    }
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-GB", {
      month: "short",
      year: "numeric",
    });
  };

  if (isLoading) {
    return <LoadingState />;
  }

  return (
    <div className="max-w-4xl mx-auto">
      <div className="mb-6 mt-8 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Experience</h1>
          <p className="text-muted-foreground mt-1">
            Manage your work and education history.
          </p>
        </div>
        <Button onClick={openCreateModal}>Add Experience</Button>
      </div>

      {experiences.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">
          No experiences yet. Click "Add Experience" to get started.
        </div>
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Title</TableHead>
              <TableHead>Organisation</TableHead>
              <TableHead>Type</TableHead>
              <TableHead>Period</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {experiences.map((experience) => (
              <TableRow key={experience.id}>
                <TableCell className="font-medium">
                  {experience.title}
                </TableCell>
                <TableCell>{experience.organization}</TableCell>
                <TableCell>{experience.type}</TableCell>
                <TableCell>
                  {formatDate(experience.startDate)} —{" "}
                  {experience.endDate
                    ? formatDate(experience.endDate)
                    : "Present"}
                </TableCell>
                <TableCell className="text-right space-x-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => openEditModal(experience)}
                  >
                    Edit
                  </Button>
                  <Button
                    variant="destructive"
                    size="sm"
                    onClick={() => handleDelete(experience.id)}
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
        <DialogContent className="max-w-lg bg-card">
          <DialogHeader>
            <DialogTitle>
              {editingId ? "Edit Experience" : "Add Experience"}
            </DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="title">
                Title <span className="text-destructive">*</span>
              </Label>
              <Input
                id="title"
                placeholder="Software Engineer"
                value={formData.title}
                onChange={(e) =>
                  setFormData({ ...formData, title: e.target.value })
                }
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="organization">
                Organisation <span className="text-destructive">*</span>
              </Label>
              <Input
                id="organization"
                placeholder="Google"
                value={formData.organization}
                onChange={(e) =>
                  setFormData({ ...formData, organization: e.target.value })
                }
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="type">
                Type <span className="text-destructive">*</span>
              </Label>
              <select
                id="type"
                value={formData.type}
                onChange={(e) =>
                  setFormData({ ...formData, type: e.target.value })
                }
                className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
                required
              >
                <option value="WORK">Work</option>
                <option value="EDUCATION">Education</option>
              </select>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="startDate">
                  Start Date <span className="text-destructive">*</span>
                </Label>
                <Input
                  id="startDate"
                  type="month"
                  value={formData.startDate}
                  onChange={(e) =>
                    setFormData({ ...formData, startDate: e.target.value })
                  }
                  max={new Date().toISOString().slice(0, 7)}
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="endDate">End Date</Label>
                <Input
                  id="endDate"
                  type="month"
                  value={formData.endDate ?? ""}
                  onChange={(e) =>
                    setFormData({ ...formData, endDate: e.target.value })
                  }
                  min={formData.startDate}
                  max={new Date().toISOString().slice(0, 7)}
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="location">Location</Label>
              <Input
                id="location"
                placeholder="London, UK"
                value={formData.location ?? ""}
                onChange={(e) =>
                  setFormData({ ...formData, location: e.target.value })
                }
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="description">Description</Label>
              <Textarea
                id="description"
                placeholder="What did you do there?"
                value={formData.description ?? ""}
                onChange={(e) =>
                  setFormData({ ...formData, description: e.target.value })
                }
              />
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
                    : "Add Experience"}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
}
