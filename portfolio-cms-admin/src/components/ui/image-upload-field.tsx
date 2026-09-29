import { useRef, useState } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { toast } from "sonner";
import { uploadFile } from "@/api/file-upload";

interface ImageUploadFieldProps {
  id: string;
  label: string;
  value: string;
  onChange: (url: string) => void;
  /** Tags the upload so it lands under this section in Files, e.g. "hero", "about", "project" */
  category: string;
  required?: boolean;
  placeholder?: string;
}

const ACCEPTED_TYPES = "image/png,image/jpeg,image/webp,image/gif";

export function ImageUploadField({
  id,
  label,
  value,
  onChange,
  category,
  required,
  placeholder = "https://example.com/photo.jpg",
}: ImageUploadFieldProps) {
  const [isUploading, setIsUploading] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setIsUploading(true);
    try {
      // isPublic: true - these images back the public portfolio site,
      // so they need to be fetchable without auth.
      const response = await uploadFile(file, category, undefined, true);
      onChange(response.data.fileUrl);
      toast.success("Image uploaded.");
    } catch (err: unknown) {
      if (err && typeof err === "object" && "response" in err) {
        const axiosErr = err as { response?: { data?: string } };
        toast.error(axiosErr.response?.data ?? "Failed to upload image.");
      } else {
        toast.error("Failed to upload image.");
      }
    } finally {
      setIsUploading(false);
      // Reset so selecting the same file again still fires onChange
      if (fileInputRef.current) fileInputRef.current.value = "";
    }
  };

  return (
    <div className="space-y-2">
      <Label htmlFor={id}>
        {label} {required && <span className="text-destructive">*</span>}
      </Label>

      {value && (
        <div className="w-28 h-28 rounded-md border border-input overflow-hidden bg-muted">
          <img
            src={value}
            alt=""
            className="w-full h-full object-cover"
            onError={(e) => {
              (e.target as HTMLImageElement).style.display = "none";
            }}
          />
        </div>
      )}

      <div className="flex gap-2">
        <Input
          id={id}
          type="url"
          placeholder={placeholder}
          value={value}
          onChange={(e) => onChange(e.target.value)}
        />
        <Button
          type="button"
          variant="outline"
          onClick={() => fileInputRef.current?.click()}
          disabled={isUploading}
        >
          {isUploading ? "Uploading..." : "Upload"}
        </Button>
      </div>

      <input
        ref={fileInputRef}
        type="file"
        accept={ACCEPTED_TYPES}
        className="hidden"
        onChange={handleFileChange}
      />

      <p className="text-xs text-muted-foreground">
        Upload an image or paste a URL. Uploads are saved under the "{category}"
        category in Files.
      </p>
    </div>
  );
}
