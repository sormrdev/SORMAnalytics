# SORMAnalytics
ASP NET pet project. Main feature "batch analysis" allow users to create unique conditions to find desired price candles of top 20 up to date stock assets.

Recommend using POSTMAN attached in this repo to interact with endpoints.

Project is in active development stage.

## Configuration and secrets

This project follows standard ASP.NET configuration precedence:

1. `appsettings.json` (committed, non-secret defaults)
2. `appsettings.{Environment}.json` (local overrides)
3. User secrets in development
4. Environment variables (highest priority)

### Local run with Docker Compose

1. Copy `.env.example` to `.env`
2. Fill real values (`Jwt__Key`, `FmpOptions__ApiKey`, etc.)
3. Run:

```bash
docker compose up --build
```
