import { useState, useEffect } from "react";
import {
  getSocialLinks,
  createSocialLink,
  updateSocialLink,
  deleteSocialLink,
} from "@/api/social-links";
import { SocialLinkDTO, SocialLinkCreateDTO } from "@/types";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
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

const emptyForm: SocialLinkCreateDTO = {
  platform: "",
  url: "",
};

export default function SocialLinks() {
  const [socialLinks, setSocialLinks] = useState<SocialLinkDTO[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState<SocialLinkCreateDTO>(emptyForm);

  useEffect(() => {
    let cancelled = false;

    const fetchSocialLinks = async () => {
      try {
        const response = await getSocialLinks();
        if (!cancelled) setSocialLinks(response.data);
      } catch {
        if (!cancelled) toast.error("Failed to load social links.");
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    };

    fetchSocialLinks();

    return () => {
      cancelled = true;
    };
  }, []);

  const openCreateModal = () => {
    setEditingId(null);
    setFormData(emptyForm);
    setIsModalOpen(true);
  };

  const openEditModal = (socialLink: SocialLinkDTO) => {
    setEditingId(socialLink.id);
    setFormData({
      platform: socialLink.platform,
      url: socialLink.url,
    });
    setIsModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this social link?")) return;

    try {
      await deleteSocialLink(id);
      setSocialLinks((prev) => prev.filter((s) => s.id !== id));
      toast.success("Social link deleted.");
    } catch {
      toast.error("Failed to delete social link.");
    }
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsSaving(true);

    try {
      if (editingId) {
        const response = await updateSocialLink(editingId, formData);
        setSocialLinks((prev) =>
          prev.map((s) => (s.id === editingId ? response.data : s)),
        );
        toast.success("Social link updated.");
      } else {
        const response = await createSocialLink(formData);
        setSocialLinks((prev) => [...prev, response.data]);
        toast.success("Social link created.");
      }
      setIsModalOpen(false);
    } catch {
      toast.error("Failed to save social link.");
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
    <div className="max-w-4xl mx-auto">
      <div className="mb-6 mt-8 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Social Links</h1>
          <p className="text-muted-foreground mt-1">
            Manage your social media and contact links.
          </p>
        </div>
        <Button onClick={openCreateModal}>Add Link</Button>
      </div>

      {socialLinks.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">
          No social links yet. Click "Add Link" to get started.
        </div>
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Platform</TableHead>
              <TableHead>URL</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {socialLinks.map((socialLink) => (
              <TableRow key={socialLink.id}>
                <TableCell className="font-medium">
                  {socialLink.platform}
                </TableCell>
                <TableCell className="text-muted-foreground text-sm">
                  <a
                    href={socialLink.url}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="hover:text-primary underline"
                  >
                    {socialLink.url}
                  </a>
                </TableCell>
                <TableCell className="text-right space-x-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => openEditModal(socialLink)}
                  >
                    Edit
                  </Button>
                  <Button
                    variant="destructive"
                    size="sm"
                    onClick={() => handleDelete(socialLink.id)}
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
              {editingId ? "Edit Social Link" : "Add Social Link"}
            </DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="platform">
                Platform <span className="text-destructive">*</span>
              </Label>
              <Input
                id="platform"
                placeholder="GitHub"
                value={formData.platform}
                onChange={(e) =>
                  setFormData({ ...formData, platform: e.target.value })
                }
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="url">
                URL <span className="text-destructive">*</span>
              </Label>
              <Input
                id="url"
                type="url"
                placeholder="https://github.com/username"
                value={formData.url}
                onChange={(e) =>
                  setFormData({ ...formData, url: e.target.value })
                }
                required
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
                    : "Add Link"}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
}
