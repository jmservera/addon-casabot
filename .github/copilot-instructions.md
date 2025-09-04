# Copilot Instructions — Home Assistant Add-on(s)

You are an assistant working **inside a Home Assistant add-on repository**. Your job is to **generate, refactor, and review** files so the add-on(s) here meet official Home Assistant standards.

## Goals (in priority order)
1) **Security-first**: least privilege, AppArmor coverage, minimal Supervisor API role, no host networking unless strictly required.
2) **Correct structure & metadata**: repo/add-on layout, config keys, schema, and images aligned with official docs.
3) **Great UX**: clear README/DOCS, proper icon/logo, changelog, and optional ingress/web UI that “just works”.
4) **Reliable builds & tests**: devcontainer flow for local testing; deterministic multi-arch image builds.

---

**Conventions**
- **Bash** entrypoint (`run.sh`) using **bashio** to parse `/data/options.json`.
- Keep persistent data in `/data`; never assume write access elsewhere unless mapped.
- Prefer small, explicit scripts over complex logic in Dockerfile.
- Follow semantic versioning in `config.yaml: version:` and maintain `CHANGELOG.md`.

---

## `config.yaml` guardrails

- Always include: `name`, `version`, `slug`, `description`, `url`, and target `arch` values actually supported by the image(s).
- Use **one** of:
  - `image: "ghcr.io/<org>/{arch}-addon-<name>"` (preferred for published images; `{arch}` token required), or
  - `build.yaml` + `Dockerfile` for local builds.
- Define **user options** and a **schema**. Keep schema strict and typed (e.g., `str`, `int`, `bool`, `list`, `float`, with `?` for optional).
- Map only what’s needed (`map:`), default to read‑only (e.g., `share:ro`), and document why any read‑write is necessary.
- If exposing a UI without ingress, use `ports` + `ports_description`. For ingress, set `ingress: true` and omit public ports unless required for non‑ingress access. Prefer `webui` only when not using ingress.
- Avoid `host_network: true`, `full_access: true`, and broad `privileged` flags. If you must use them, add explicit justification in `DOCS.md` and AppArmor rules to narrow scope.
- Add `homeassistant_api: true` or `hassio_api: true` only when you actually call those APIs. Also add the **minimal** `hassio_role` required.
- Set `init: false` unless the add-on truly needs a full init system.
- Include `watchdog` URL when a health endpoint exists.

Provide an **example minimal**:

```yaml
name: "Example add-on"
version: "1.0.0"
slug: example
description: "Short, imperative description"
url: "https://github.com/<org>/<repo>/tree/main/example"
arch: [armhf, armv7, aarch64, amd64, i386]

init: false
map:
  - share:ro

options:
  message: "Hello world..."
schema:
  message: "str?"

image: "ghcr.io/<org>/{arch}-addon-example"
```

---

## Dockerfile guardrails

- Start from the Supervisor-provided base via `ARG BUILD_FROM` / `FROM $BUILD_FROM`.
- Install only what’s needed; prefer Alpine packages.
- Copy `run.sh`, `chmod +x`, and use `CMD ["/run.sh"]`.
- Add required labels when not using the official builder pipeline:
  - `io.hass.version`, `io.hass.type=addon`, `io.hass.arch=...`.
- Keep layers small; avoid cache-busting unless necessary.

---

## Communication patterns

- **Home Assistant Core API (HTTP)**: call via `http://supervisor/core/api/` using `SUPERVISOR_TOKEN` and `homeassistant_api: true`.  
- **Home Assistant WebSocket**: `ws://supervisor/core/websocket` with `SUPERVISOR_TOKEN`.  
- **Supervisor API**: `http://supervisor/` with `hassio_api: true` and the minimal `hassio_role`.  
- **Internal addressing**: Use internal DNS name/alias (repo+slug based). Replace `_` with `-` for hostnames. Prefer aliases if an add-on uses host networking.  
- **STDIN**: Support tasks via `hassio.addon_stdin` where appropriate.

Always inject `SUPERVISOR_TOKEN` from env and send it as a Bearer token. Never hardcode tokens or base URLs.

---

## Ingress & Web UI

- Prefer **ingress** for UIs so users don’t need to manage ports.
- Ensure the served app uses **relative paths** (avoid absolute `/…` links) so it renders correctly under ingress paths.
- If you also expose a direct port (non‑ingress), document why and describe the security implications.

---

## Security checklist (block PRs if violated)

- [ ] AppArmor: provide a focused `apparmor.txt` (deny-by-default; permit only what’s needed).  
- [ ] No `host_network`, no `full_access`, and no broad `privileged` unless justified.  
- [ ] Read‑only mounts by default (`map:`).  
- [ ] Minimal `hassio_role`; remove unneeded API access.  
- [ ] No embedded credentials; use HA auth (`auth_api`) or Supervisor-provided headers for ingress sessions.  
- [ ] Consider **Codenotary CAS** signing for published images.  
- [ ] Document all elevated permissions in `DOCS.md` with mitigation notes.

---

## Testing & local dev

- Provide a `.devcontainer/` with the standard configuration so “**Start Home Assistant**” boots Supervisor+HA and auto-registers local add-ons.  
- Include `.vscode/tasks.json` for convenience tasks.  
- When building locally (outside devcontainer), prefer the **official Builder** image to produce multi-arch images and run test builds.  
- Ensure `image:` is commented out in `config.yaml` when you want Supervisor to **build locally** from the Dockerfile.

---

## Publishing guidance

- Prefer **pre-built images** per arch on GHCR or Docker Hub. Use `{arch}` in the `image:` name.
- Keep the default branch in sync with the **latest** release tag. Build in a separate branch/PR, then merge after pushing images.
- Provide release notes in `CHANGELOG.md`; bump `version` in `config.yaml`.

---

## Presentation

- `README.md`: short intro, key features, configuration quick-start, support links.  
- `DOCS.md`: full options reference (with examples), permissions rationale, troubleshooting, migration/backup notes, license.  
- `icon.png` (PNG, square, ~128×128) and `logo.png` (PNG, ~250×100).  
- Offer **stable** and optional **canary/beta** via branches; document how to add each repo variant.

---

## Repository metadata (root)

Add a `repository.yaml` at the repo root:

```yaml
name: "My Add-ons"
url: "https://github.com/<org>/<repo>"
maintainer: "Your Name <you@example.com>"
```

Users install via Supervisor → Add-on Store → Repositories.

---

## PR review rubric (use as comments)

- **Config & schema**: valid keys, types, `{arch}` images, minimal maps/ports.  
- **Security**: AppArmor present, least privilege, no host net, minimal roles.  
- **Comms**: API proxies used correctly; token handling; no hardcoded endpoints.  
- **UX**: ingress-ready UI, README/DOCS complete, icons/logos present.  
- **Build/Test**: devcontainer works; builder args OK; CI scripts (if any) pass.  
- **Versioning**: version bump + changelog.  

If any “musts” fail, request changes with exact fixes.
