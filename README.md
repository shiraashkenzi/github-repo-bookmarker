# GitHub Repo Bookmarker

A small app for searching GitHub repositories and bookmarking the ones you want to keep.

## Tech stack

- **Frontend:** Angular 21 (standalone components), Signals, Bootstrap
- **Backend:** .NET 10 Minimal API, JWT authentication, in-memory session, GitHub API integration

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Node.js 20.19+ (or 22.12+ / 24+) and npm

## Running the app

Start the backend first, then the frontend, each in its own terminal.

```bash
# Backend (http://localhost:5080)
cd backend/GitApp.Api
dotnet run

# Frontend (http://localhost:4200)
cd frontend
npm install
npm start
```

Then open http://localhost:4200 and log in with one of the demo users:

| Username | Password |
|---|---|
| `demo` | `demo123` |
| `alice` | `alice123` |

## Configuration

Backend settings are in `backend/GitApp.Api/appsettings.json`:

- `Jwt:Key`: the JWT signing key. It's a development key only, so replace it before deploying anywhere.
- `Jwt:Issuer`: the JWT issuer.
- `Jwt:ExpiryMinutes`: how long a login lasts (60 by default).
- `DemoUsers`: the users who can log in.
- `GitHub:BaseUrl`: the GitHub API address.

In development, `frontend/proxy.conf.json` forwards `/api` calls to the backend on port 5080, so no CORS setup is needed.

## Features

- Log in and log out with a demo user
- Search GitHub repositories
- Bookmark and remove repositories (each bookmark stores the full GitHub repository object in the user's session)
- View your bookmarks on a separate page
- Pages are protected, so logged-out users are sent to the login page

## Project structure

```
backend/GitApp.Api/
  Program.cs        App setup and JWT authentication
  Endpoints/        Auth, repo search and bookmark endpoints
  Services/         Token, session and GitHub services

frontend/src/app/
  auth/             Auth service, interceptor and guard
  login/ search/ bookmarks/   Pages
  repo-card/        Repository card component
```

Sessions are kept in memory, so they are cleared when the backend restarts.
