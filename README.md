# PortfolioCMS

A CMS specifically designed for portfolios, just to futureproof your laziness (and for my coding practice).

Build it once, deploy it, forget about it. When you land a new job, finish a side project, or just want to update your bio — log in, change the content, done. No touching code. No redeploying. No excuses.

## What It Is

PortfolioCMS is a headless CMS with a built-in admin panel, designed specifically for developer portfolios. It gives you a clean API your portfolio frontend can consume, and an admin site where you manage everything without ever opening your editor.

It's not trying to be WordPress. It's not trying to be Contentful. It's trying to be the thing you set up once and never think about again.

## What's Included

- **REST API** (.NET 9) - serves all your portfolio content
- **Admin Panel** (React + Vite) - manage your content without touching code
- **JWT Authentication** - only you can edit your content
- **Built-in sections** for the stuff every portfolio needs
- **Custom sections** for everything else

## Built-in Sections

| Section    | Type     | Description                          |
| ---------- | -------- | ------------------------------------ |
| Hero       | Single   | Name, title, subtitle, profile image |
| About      | Single   | Header and bio paragraph             |
| Experience | Multiple | Work and education timeline          |
| Projects   | Multiple | Your work, with featured support     |
| Contact    | Single   | Email and resume (PDF + DOCX)        |
| Custom     | Multiple | Build whatever section you need      |

All sections are optional. Fill in what you want, leave out what you don't. If a section has no content, it simply returns a 404 - your frontend handles the rest.

## Tech Stack

### Backend

- .NET 10
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Authentication
- MailKit (SMTP email - contact form + password reset)

### Admin Panel

- React + Vite
- TypeScript
- Tailwind CSS + shadcn/ui + Radix UI
- React Router
- Axios

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (or LocalDB for development)
- Node.js 18+

### Backend Setup

```bash
# Clone the repo
git clone https://github.com/yourusername/PortfolioCMS.git
cd PortfolioCMS/PortfolioCMS

# Run migrations
dotnet ef database update

# Start the API
dotnet run
```

> **`appsettings.json` is tracked in this repo and stays that way** — it
> only holds harmless, non-sensitive defaults (CORS origins, JWT
> issuer/audience, SMTP host/port). Every real secret (JWT signing key,
> database connection string, email password) is supplied as an
> **environment variable** on the host instead, and is never written into
> this file. ASP.NET Core merges `appsettings.json` with environment
> variables at startup, with env vars taking precedence, so this works
> without any local override file. Nested keys use a double underscore,
> e.g.:
>
> ```
> ConnectionStrings__DefaultConnection=Server=...;Database=...;User Id=...;Password=...
> JwtSettings__SecretKey=<a long random string>
> EmailSettings__Password=<smtp password>
> ```
>
> Set these however your host manages env vars — a docker-compose
> `environment:`/`env_file:` block, a systemd unit's `Environment=`, or
> your platform's secrets/config UI.

### First Time Setup

Once the API is running, register your admin account:

```
POST /api/auth/register
{
  "email": "you@example.com",
  "username": "yourname",
  "password": "YourPassword1"
}
```

That's it. You're in. Use the token you get back to authenticate all future requests.

> You only need to register once. After that, just use `/api/auth/login`.

### Email Setup (contact form + password reset)

`EmailSettings` in `appsettings.json` controls what account the API sends
mail from — used both for the portfolio's contact form and for password
reset emails. It works with Gmail out of the box, or any SMTP provider
(Outlook, a transactional service, your own mail server) by changing
`SmtpHost`/`SmtpPort` accordingly.

**Using Gmail:**

1. Turn on [2-Step Verification](https://myaccount.google.com/security) on
   the Google account you want to send from — required for the next step.
2. Go to [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords),
   sign in, and generate an app password (name it whatever you like, e.g.
   "PortfolioCMS"). Google shows a 16-character code once — copy it.
3. Set these as environment variables on your host (docker-compose,
   systemd, or your platform's config UI — see Backend Setup above):
   ```
   EmailSettings__SenderEmail=you@gmail.com
   EmailSettings__SenderName=Your Name
   EmailSettings__Password=your-16-char-app-password
   ```
   `SmtpHost`/`SmtpPort` only need overriding if you're not using Gmail —
   the `appsettings.json` defaults (`smtp.gmail.com`, `587`) already match.

> Google's own docs now describe app passwords as "not recommended" in
> favor of OAuth-based "Sign in with Google" — that guidance targets
> end-user-facing apps, though. For a backend service sending its own mail
> (this one), an app password is still the standard approach; full OAuth2
> for SMTP is considerably more setup for no real benefit here.

## API Overview

All content endpoints are public (GET) so your portfolio frontend can read freely. Write operations (POST, PUT, DELETE) require a Bearer token.

```
# Public
GET  /api/projects
GET  /api/hero
GET  /api/about
GET  /api/experience
POST /api/contact       # portfolio contact form - rate-limited, sends email

# Protected (requires token)
POST   /api/projects
PUT    /api/projects/{id}
DELETE /api/projects/{id}
```

Full API documentation coming soon.

## Building Your Portfolio Frontend

PortfolioCMS is headless - it doesn't care what your frontend looks like. Use whatever you want:

- Next.js
- Astro
- plain HTML
- whatever framework you're into this week

Point it at your API, consume the endpoints, build your theme. That's the whole idea — you change the theme without touching the CMS, and you update content without touching the theme.

## Roadmap

- [x] Project Section API routes
- [x] Login API routes
- [x] Rest of the section API routes
- [x] Admin panel (React + Vite)
- [x] File upload support (hero image, resume)
- [ ] Custom sections with block types (text, list, link, file)
- [ ] Template system for custom sections
- [ ] Example portfolio frontend (Next.js)
- [ ] One-click deploy guides (Railway, Render, Azure)

## License

MIT - use it, fork it, build your portfolio with it. Just don't blame me if you're still lazy.
