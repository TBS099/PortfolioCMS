import { useState } from "react";
import { useAuth } from "../../hooks/useAuth";
import { register } from "../../api/auth";
import { RegisterDTO } from "../../types";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card";

export default function Register() {
  const { setIsAuthenticated, setRequiresSetup } = useAuth();
  const [formData, setFormData] = useState<RegisterDTO>({
    email: "",
    password: "",
    username: "",
  });
  const [confirmEmail, setConfirmEmail] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const emailsMatch = formData.email === confirmEmail;
  const passwordsMatch = formData.password === confirmPassword;

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!emailsMatch) {
      toast.error("Emails do not match.");
      return;
    }

    if (!passwordsMatch) {
      toast.error("Passwords do not match.");
      return;
    }

    setIsLoading(true);

    try {
      await register(formData);
      toast.success("Account created successfully.");
      setRequiresSetup(false);
      setTimeout(() => setIsAuthenticated(true), 1500);
    } catch (err: unknown) {
      if (err && typeof err === "object" && "response" in err) {
        const axiosErr = err as {
          response?: {
            data?:
              | {
                  title?: string;
                  errors?: Record<string, string[]>;
                }
              | string
              | string[];
          };
        };

        const data = axiosErr.response?.data;

        if (typeof data === "string") {
          toast.error(data);
        } else if (Array.isArray(data)) {
          // Identity's CreateAsync failures come back as a plain string array
          toast.error(data.join(" "));
        } else if (data?.errors) {
          // Flatten all validation errors into one string
          const messages = Object.values(data.errors).flat();
          toast.error(messages.join(" "));
        } else if (data?.title) {
          toast.error(data.title);
        } else {
          toast.error("Registration failed.");
        }
      } else {
        toast.error("Registration failed.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle className="text-2xl font-bold text-center">
            Portfolio CMS
          </CardTitle>
          <CardDescription className="text-center">
            Create your admin account to get started
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="username">Username</Label>
              <Input
                id="username"
                type="text"
                placeholder="yourname"
                value={formData.username}
                onChange={(e) =>
                  setFormData({ ...formData, username: e.target.value })
                }
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                placeholder="you@example.com"
                value={formData.email}
                onChange={(e) =>
                  setFormData({ ...formData, email: e.target.value })
                }
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="confirmEmail">Confirm Email</Label>
              <Input
                id="confirmEmail"
                type="email"
                placeholder="you@example.com"
                value={confirmEmail}
                onChange={(e) => setConfirmEmail(e.target.value)}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="password">Password</Label>
              <Input
                id="password"
                type="password"
                placeholder="••••••••••"
                value={formData.password}
                onChange={(e) =>
                  setFormData({ ...formData, password: e.target.value })
                }
                required
              />
              <p className="text-xs text-muted-foreground">
                Min 10 characters, must include uppercase, digit and special
                character
              </p>
            </div>
            <div className="space-y-2">
              <Label htmlFor="confirmPassword">Confirm Password</Label>
              <Input
                id="confirmPassword"
                type="password"
                placeholder="••••••••••"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                required
              />
            </div>
            <Button type="submit" className="w-full" disabled={isLoading}>
              {isLoading ? "Creating account..." : "Create account"}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
