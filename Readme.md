# Pine & Pour

This repository contains:

- `PinePour.Api`: ASP.NET Core API
- `PinePour.Web`: React + Vite web app
- `PinePour.Mobile`: Expo mobile app
- `PinePour.Tests`: automated tests

The easiest way to run the project locally on macOS or Windows is Docker Compose.

## 1. Run the API with Docker

This starts:

- PostgreSQL
- the ASP.NET API
- the Vite web app

From the repo root, run:

```bash
cp .env.example .env
# Set POSTGRES_PASSWORD in .env first
docker compose up --build -d
```

Then open:

- `http://localhost:8080`
- `http://localhost:8080/swagger/index.html`
- `http://localhost:5173`

Useful commands:

```bash
docker compose logs -f api
docker compose down
```

Note: in the current `docker-compose.yml`, the `api` service depends on both `db` and `web`, so starting the API through Docker Compose brings those services up as well.

## 2. Run API without Docker

If you prefer to run the API directly with `dotnet`, set the environment variable `ConnectionStrings__DataContext` to your Postgres connection string (for Supabase on Render, use the Supabase **Session pooler** string), then run:

```bash
cd PinePour.Api
ConnectionStrings__DataContext="<your-postgres-connection-string>" dotnet run
```

Then open:

- `http://localhost:5173`
- the API URL shown by `dotnet run`

If you use `zsh` on macOS, the command above works as written. If you prefer another shell, set the environment variable using that shell's syntax.


## 3. Mobile App

The mobile app is optional and separate from the Docker stack.

From the repo root:

```bash
cd PinePour.Mobile
npm install
npm start
or npm run android

```

Then choose one of the Expo targets shown in the terminal.
