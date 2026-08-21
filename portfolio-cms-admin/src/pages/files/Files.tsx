import { useState, useEffect, useRef } from "react";
import {
  getFiles,
  uploadFile,
  deleteFile,
  updateVisibility,
} from "@/api/file-upload";
import { FileUploadDTO } from "@/types";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Checkbox } from "@/components/ui/checkbox";
import { LoadingState } from "@/components/ui/loading-state";

const CATEGORIES = ["resume", "cover-letter", "certificate", "image", "other"];

const formatFileSize = (bytes: number) => {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
};

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString("en-GB", {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
};

export default function Files() {
  const [files, setFiles] = useState<FileUploadDTO[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isPublic, setIsPublic] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isUploading, setIsUploading] = useState(false);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [category, setCategory] = useState("resume");
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [displayName, setDisplayName] = useState("");

  useEffect(() => {
    let cancelled = false;

    const fetchFiles = async () => {
      try {
        const response = await getFiles();
        if (!cancelled) setFiles(response.data);
      } catch {
        if (!cancelled) toast.error("Failed to load files.");
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    };

    fetchFiles();
    return () => {
      cancelled = true;
    };
  }, []);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) setSelectedFile(file);
  };

  const handleUpload = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!selectedFile) {
      toast.error("Please select a file.");
      return;
    }

    setIsUploading(true);

    try {
      const response = await uploadFile(
        selectedFile,
        category,
        displayName || undefined,
        isPublic,
      );
      setFiles((prev) => [response.data, ...prev]);
      toast.success("File uploaded successfully.");
      setIsModalOpen(false);
      setSelectedFile(null);
      setCategory("resume");
      setDisplayName("");
      setIsPublic(true);
      if (fileInputRef.current) fileInputRef.current.value = "";
    } catch (err: unknown) {
      if (err && typeof err === "object" && "response" in err) {
        const axiosErr = err as { response?: { data?: string } };
        toast.error(axiosErr.response?.data ?? "Failed to upload file.");
      } else {
        toast.error("Failed to upload file.");
      }
    } finally {
      setIsUploading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this file?")) return;

    try {
      await deleteFile(id);
      setFiles((prev) => prev.filter((f) => f.id !== id));
      toast.success("File deleted.");
    } catch {
      toast.error("Failed to delete file.");
    }
  };

  if (isLoading) {
    return <LoadingState />;
  }

  return (
    <div className="max-w-4xl mx-auto">
      <div className="mb-6 mt-8 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Files</h1>
          <p className="text-muted-foreground mt-1">
            Manage your uploaded files — CVs, cover letters, images and more.
          </p>
        </div>
        <Button onClick={() => setIsModalOpen(true)}>Upload File</Button>
      </div>

      {files.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">
          No files uploaded yet. Click "Upload File" to get started.
        </div>
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead>Category</TableHead>
              <TableHead>Size</TableHead>
              <TableHead>Uploaded</TableHead>
              <TableHead>Visibility</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {files.map((file) => (
              <TableRow key={file.id}>
                <TableCell className="font-medium">
                  <a
                    href={file.fileUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="hover:text-primary underline"
                  >
                    {file.displayName || file.originalName}
                  </a>
                </TableCell>
                <TableCell>
                  <span className="text-xs bg-secondary text-secondary-foreground px-2 py-0.5 rounded-full capitalize">
                    {file.category.replace("-", " ")}
                  </span>
                </TableCell>
                <TableCell className="text-muted-foreground text-sm">
                  {formatFileSize(file.fileSize)}
                </TableCell>
                <TableCell className="text-muted-foreground text-sm">
                  {formatDate(file.uploadedAt)}
                </TableCell>
                <TableCell>
                  <span
                    className={`text-xs px-2 py-0.5 rounded-full ${
                      file.isPublic
                        ? "bg-green-500/10 text-green-500"
                        : "bg-muted text-muted-foreground"
                    }`}
                  >
                    {file.isPublic ? "Public" : "Private"}
                  </span>
                </TableCell>
                <TableCell className="text-right space-x-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={async () => {
                      try {
                        const response = await updateVisibility(
                          file.id,
                          !file.isPublic,
                        );
                        setFiles((prev) =>
                          prev.map((f) =>
                            f.id === file.id ? response.data : f,
                          ),
                        );
                        toast.success(
                          file.isPublic
                            ? "File set to private."
                            : "File set to public.",
                        );
                      } catch {
                        toast.error("Failed to update visibility.");
                      }
                    }}
                  >
                    {file.isPublic ? "Make Private" : "Make Public"}
                  </Button>
                  <Button variant="outline" size="sm" asChild>
                    <a
                      href={file.fileUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                    >
                      View
                    </a>
                  </Button>
                  <Button
                    variant="destructive"
                    size="sm"
                    onClick={() => handleDelete(file.id)}
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
        <DialogContent className="max-w-md bg-card">
          <DialogHeader>
            <DialogTitle>Upload File</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleUpload} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="category">Category</Label>
              <select
                id="category"
                value={category}
                onChange={(e) => setCategory(e.target.value)}
                className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
              >
                {CATEGORIES.map((cat) => (
                  <option key={cat} value={cat}>
                    {cat
                      .replace("-", " ")
                      .replace(/\b\w/g, (l) => l.toUpperCase())}
                  </option>
                ))}
              </select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="displayName">Display Name</Label>
              <Input
                id="displayName"
                placeholder="My CV 2024"
                value={displayName}
                onChange={(e) => setDisplayName(e.target.value)}
              />
              <p className="text-xs text-muted-foreground">
                Leave blank to use the original filename.
              </p>
            </div>

            <div className="flex items-center gap-2">
              <Checkbox
                id="isPublic"
                checked={isPublic}
                onCheckedChange={(checked) => setIsPublic(checked === true)}
              />
              <Label htmlFor="isPublic" className="cursor-pointer">
                Make public
              </Label>
              <p className="text-xs text-muted-foreground">
                Public files are visible on your portfolio.
              </p>
            </div>

            <div className="space-y-2">
              <Label htmlFor="file">File</Label>
              <Input
                id="file"
                type="file"
                ref={fileInputRef}
                onChange={handleFileChange}
                accept=".pdf,.doc,.docx,.jpg,.jpeg,.png,.webp,.gif"
                required
              />
              <p className="text-xs text-muted-foreground">
                Allowed: PDF, Word, JPEG, PNG, WebP, GIF. Max 10MB.
              </p>
            </div>

            {selectedFile && (
              <p className="text-sm text-muted-foreground">
                Selected: {selectedFile.name} (
                {formatFileSize(selectedFile.size)})
              </p>
            )}

            <div className="flex justify-end gap-2 pt-2">
              <Button
                type="button"
                variant="outline"
                onClick={() => {
                  setIsModalOpen(false);
                  setSelectedFile(null);
                  if (fileInputRef.current) fileInputRef.current.value = "";
                }}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={isUploading}>
                {isUploading ? "Uploading..." : "Upload"}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
}
