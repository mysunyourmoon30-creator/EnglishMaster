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
ENGLISHMASTER_PRODUCTION_SQL_EDITION=<licensed production edition, e.g. Standard>
ENGLISHMASTER_PRODUCTION_API_ALLOWED_HOSTS=api.your-domain.example;englishmaster-production-api
ENGLISHMASTER_PRODUCTION_WEB_ALLOWED_HOSTS=app.your-domain.example
# Optional when the defaults conflict with the VPS/VPN network:
# ENGLISHMASTER_PRODUCTION_NETWORK_SUBNET=172.30.0.0/24
# ENGLISHMASTER_PRODUCTION_PROXY_IP=172.30.0.10
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

Compose assigns Caddy the configured static private IP; API and Web trust only
that address for `X-Forwarded-For` and `X-Forwarded-Proto`. Keep API and Web
ports unpublished so untrusted clients cannot bypass Caddy.

### 6. Place the reviewed release artifacts

Download the `englishmaster-release-build` artifact produced by the tagged
release workflow and place its `artifacts` directory next to
`docker-compose.production.yml`. Verify the artifact provenance and checksum
before use.

### 7. Start SQL Server

```bash
docker compose -f docker-compose.production.yml up -d englishmaster-production-sqlserver
docker compose -f docker-compose.production.yml ps
```

Wait until SQL Server reports healthy.

### 8. Back up and apply database migrations

For an existing database, complete the backup checklist first. The API does
not auto-apply migrations in Production. Make the reviewed Linux bundle
executable and run the one-shot migration service on the private Compose
network:

```bash
chmod 0555 artifacts/migrations/englishmaster-migrate
docker compose -f docker-compose.production.yml --profile operations run --rm \
  englishmaster-production-migrations
```

The migration service receives the connection string from Compose secrets and
does not expose SQL Server on a host port. Stop the release if it exits
non-zero.

### 9. Start the application

```bash
docker compose -f docker-compose.production.yml up -d --build
docker compose -f docker-compose.production.yml ps
```

Wait until all application services report healthy.

### 10. Verify

```bash
curl https://api.your-domain.example/health/ready
curl https://app.your-domain.example/health/live
```

Both should return `Healthy`. From a trusted machine with PowerShell, run the
packaged automated gate:

```powershell
$env:ENGLISHMASTER_SMOKE_ADMIN_EMAIL = "<production-admin-email>"
$env:ENGLISHMASTER_SMOKE_ADMIN_PASSWORD = "<production-admin-password>"
./artifacts/release-tools/Invoke-EnglishMasterReleaseSmoke.ps1 `
  -ApiBaseUrl "https://api.your-domain.example" `
  -WebBaseUrl "https://app.your-domain.example" `
  -RequireAuthenticatedChecks
Remove-Item Env:ENGLISHMASTER_SMOKE_ADMIN_EMAIL
Remove-Item Env:ENGLISHMASTER_SMOKE_ADMIN_PASSWORD
```

Then open the Web app in a browser and complete the manual UI checks.

### 11. Rotate bootstrap credentials

Once you've confirmed SuperAdmin login works, remove `ENGLISHMASTER_PRODUCTION_SUPERADMIN_EMAIL` / `ENGLISHMASTER_PRODUCTION_SUPERADMIN_PASSWORD` from `.env` and restart the API container, per `docs/deployment/production-database.md`'s bootstrap-credential-rotation guidance.

### 12. Record the deployment

Fill in `docs/release/v0.3.0-production-deployment-record.md`'s Environment table (Production URL, deployment revision, timestamps) and Deployment Steps table with what actually happened.

## Logs

Structured logs (Serilog) go to both the container's console (`docker compose -f docker-compose.production.yml logs -f`) and a durable rolling file volume at `/app/logs` inside each container — one file per day, 14-day retention, mounted to `englishmaster-production-api-logs` / `englishmaster-production-web-logs` so they survive container restarts.

API and Web Data Protection keys use separate durable named volumes. Restrict
Docker volume access and include these volumes in protected operational
backups; loss of a key volume invalidates that application's protected
cookies/tokens. Do not copy the API key ring to the Web app or vice versa.

## Backups

Before the first production migration and on an ongoing schedule, follow `docs/operations/database-backup-restore.md` and `docs/operations/backup-restore-checklist.md`. `docker exec` into `englishmaster-production-sqlserver` to run `BACKUP DATABASE`, or use `docker cp` to pull the volume contents off the container.

## Rollback

If something goes wrong, `docker compose -f docker-compose.production.yml down` and restore from the most recent backup per `docs/release/v0.3.0-rollback-plan.md`. Rollback owner: `chotikku` (see `docs/release/v0.3.0-production-go-live-checklist.md`).
