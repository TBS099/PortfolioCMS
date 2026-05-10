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

- .NET 9
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Authentication

### Admin Panel

- React + Vite
- (in progress)


## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server (or LocalDB for development)
- Node.js 18+

### Backend Setup

```bash
# Clone the repo
git clone https://github.com/yourusername/PortfolioCMS.git
cd PortfolioCMS/PortfolioCMS

# Update appsettings.json with your connection string and JWT secret
# See appsettings.example.json for reference

# Run migrations
dotnet ef database update

# Start the API
dotnet run
```

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


## API Overview

All content endpoints are public (GET) so your portfolio frontend can read freely. Write operations (POST, PUT, DELETE) require a Bearer token.

```
# Public
GET /api/projects
GET /api/hero
GET /api/about
GET /api/experience

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
- [ ] Rest of the section API routes
- [ ] Admin panel (React + Vite)
- [ ] File upload support (hero image, resume)
- [ ] Custom sections with block types (text, list, link, file)
- [ ] Template system for custom sections
- [ ] Example portfolio frontend (Next.js)
- [ ] One-click deploy guides (Railway, Render, Azure)

## License

MIT - use it, fork it, build your portfolio with it. Just don't blame me if you're still lazy.
