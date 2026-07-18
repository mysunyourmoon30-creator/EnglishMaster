# Production Deployment: Linux VPS + Docker Compose

## Status

`Runbook Prepared - Not Yet Executed`

## Why This Path

Chosen 2026-07-18 (see `docs/release/v0.3.0-production-deployment-record.md`). Reasoning: file storage is local-path only (`docs/deployment/production-file-storage.md`), Blazor Server needs long-lived connections, background workers need a continuously-running process, and the stack requires SQL Server specifically — a VPS running the existing Docker Compose setup satisfies all of this with no new infrastructure work, at flat/predictable cost. The Docker Compose staging re-validation issue recorded in `docs/release/v0.3.0-production-deployment-record.md` was a BIOS/Hyper-V virtualization problem specific to the local Windows development machine; it does not apply to Docker running natively on a Linux VPS.

This runbook uses `docker-compose.production.yml` (repo root) and `Caddyfile.example` (repo root, copy to `Caddyfile` and edit before use).

## Prerequisites

- A VPS running Ubuntu 22.04 or later, with at least 2 vCPU / 4 GB RAM (SQL Server needs headroom). Any provider works (DigitalOcean, Hetzner, Linode, etc.) — this runbook is provider-agnostic.
- A domain name you control, with the ability to add DNS A records.
- SSH access to the VPS as a non-root user with `sudo`.

## Steps

### 1. DNS

Point two A records at the VPS's public IP:

- `app.your-domain.example` → the Web app
- `api.your-domain.example` → the API

(Use your real domain — these are placeholders matching `Caddyfile.example`.)

### 2. Install Docker on the VPS

```bash
curl -fsSL https://get.docker.com | sudo sh
sudo usermod -aG docker $USER
# log out and back in for the group change to take effect
docker compose version   # confirms the Compose plugin is present
```

### 3. Get the code onto the server

```bash
git clone <this-repo-url> englishmaster
cd englishmaster/EnglishMaster/EnglishMaster   # adjust to wherever docker-compose.production.yml lives after clone
```

### 4. Configure secrets

Create a `.env` file next to `docker-compose.production.yml` (never commit this):

```bash
ENGLISHMASTER_PRODUCTION_SQL_PASSWORD=<generate a strong password>
ENGLISHMASTER_PRODUCTION_ALLOWED_HOSTS=app.your-domain.example;api.your-domain.example
ENGLISHMASTER_PRODUCTION_SUPERADMIN_EMAIL=<your admin email, temporary>
ENGLISHMASTER_PRODUCTION_SUPERADMIN_PASSWORD=<a strong temporary password>
# Email (optional at first launch — leave Email__Provider at Development if not ready):
ENGLISHMASTER_PRODUCTION_EMAIL_PROVIDER=Smtp
ENGLISHMASTER_PRODUCTION_EMAIL_FROM=<your-from-address>
ENGLISHMASTER_PRODUCTION_SMTP_HOST=smtp.gmail.com
ENGLISHMASTER_PRODUCTION_SMTP_PORT=587
ENGLISHMASTER_PRODUCTION_SMTP_USERNAME=<smtp username>
ENGLISHMASTER_PRODUCTION_SMTP_PASSWORD=<smtp app password>
ENGLISHMASTER_PRODUCTION_ALERT_EMAIL=<where SystemHealthWorker alerts should go>
```

See `docs/deployment/production-environment-variables.md` for what each value does and `docs/operations/email-configuration.md` for Gmail SMTP setup specifically.

### 5. Set up the reverse proxy

```bash
cp Caddyfile.example Caddyfile
# edit Caddyfile: replace app.your-domain.example / api.your-domain.example with your real domains
```

Caddy requests and renews TLS certificates automatically via Let's Encrypt — no manual certbot step.

### 6. Start everything

```bash
docker compose -f docker-compose.production.yml up -d --build
docker compose -f docker-compose.production.yml ps   # wait until all services show healthy
```

### 7. Apply database migrations

The API does not auto-apply migrations on startup in Production (by design — see `docs/deployment/production-database.md`). Apply them explicitly:

```bash
docker compose -f docker-compose.production.yml exec englishmaster-production-api \
  dotnet ef database update --project /src/EnglishMaster.Infrastructure --startup-project /src/EnglishMaster.Api
```

If the container image doesn't include the `dotnet ef` tooling, run migrations from a machine with the repo and the .NET SDK instead, pointed at the production connection string via `ConnectionStrings__DefaultConnection`.

### 8. Verify

```bash
curl https://api.your-domain.example/health/ready
curl https://app.your-domain.example/health/live
```

Both should return `Healthy`. Then open `https://app.your-domain.example` in a browser and log in with the bootstrap SuperAdmin credentials from step 4.

### 9. Rotate bootstrap credentials

Once you've confirmed SuperAdmin login works, remove `ENGLISHMASTER_PRODUCTION_SUPERADMIN_EMAIL` / `ENGLISHMASTER_PRODUCTION_SUPERADMIN_PASSWORD` from `.env` and restart the API container, per `docs/deployment/production-database.md`'s bootstrap-credential-rotation guidance.

### 10. Record the deployment

Fill in `docs/release/v0.3.0-production-deployment-record.md`'s Environment table (Production URL, deployment revision, timestamps) and Deployment Steps table with what actually happened.

## Logs

Structured logs (Serilog) go to both the container's console (`docker compose -f docker-compose.production.yml logs -f`) and a durable rolling file volume at `/app/logs` inside each container — one file per day, 14-day retention, mounted to `englishmaster-production-api-logs` / `englishmaster-production-web-logs` so they survive container restarts.

## Backups

Before the first production migration and on an ongoing schedule, follow `docs/operations/database-backup-restore.md` and `docs/operations/backup-restore-checklist.md`. `docker exec` into `englishmaster-production-sqlserver` to run `BACKUP DATABASE`, or use `docker cp` to pull the volume contents off the container.

## Rollback

If something goes wrong, `docker compose -f docker-compose.production.yml down` and restore from the most recent backup per `docs/release/v0.3.0-rollback-plan.md`. Rollback owner: `chotikku` (see `docs/release/v0.3.0-production-go-live-checklist.md`).
