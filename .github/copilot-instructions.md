# addon-casabot
Home Assistant add-on repository containing "Casa Bot" - an LLM-enabled bot to interact with Home Assistant's MCP server.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively
- Bootstrap and test the repository:
  - Ensure Docker is available: `docker --version`
  - Test Home Assistant Builder access: `docker run --rm homeassistant/amd64-builder --help`
  - Build the add-on: `docker run --rm -v /var/run/docker.sock:/var/run/docker.sock -v "$(pwd)":/data homeassistant/amd64-builder --test --amd64 --target /data/casabot` -- takes 8-10 seconds. NEVER CANCEL. Set timeout to 30+ seconds.
  - Multi-arch builds (ARM): `docker run --rm -v /var/run/docker.sock:/var/run/docker.sock -v "$(pwd)":/data homeassistant/amd64-builder --test --armv7 --target /data/casabot` -- takes 8-10 seconds. NEVER CANCEL. Set timeout to 30+ seconds.
- Run linting and validation:
  - YAML linting: `yamllint casabot/config.yaml` -- expects document start marker and newline at end
  - Shell script linting: `shellcheck casabot/run.sh` -- will warn about bashio shebang (expected)
  - Python validation: `python3 -c "import yaml; print('YAML parsing works')"`
- Test functionality:
  - Run container: `docker run --rm -p 8000:8000 -d --name casabot-test ghcr.io/jmservera/amd64-addon-casabot:0.1.5`
  - Test HTTP response: `curl -s -o /dev/null -w "%{http_code}" http://localhost:8000/` -- should return 200
  - Verify content: `curl -s http://localhost:8000/ | head -10` -- should show directory listing HTML
  - Stop container: `docker stop casabot-test`

## Validation
- Always manually validate any new code by building the add-on and testing HTTP functionality.
- ALWAYS run through at least one complete build-test-run scenario after making changes.
- Build both AMD64 and ARM architectures to ensure cross-platform compatibility works.
- Always test the web interface by running the container and making HTTP requests to port 8000.
- Always run `yamllint casabot/config.yaml` and `shellcheck casabot/run.sh` before you are done or the CI (.github/workflows/build.yaml) may fail.

## Common tasks
The following are outputs from frequently run commands. Reference them instead of viewing, searching, or running bash commands to save time.

### Repository root
```
ls -la
total 36
drwxr-xr-x 6 runner docker 4096 Sep  2 11:46 .
drwxr-xr-x 3 runner docker 4096 Sep  2 11:46 ..
drwxr-xr-x 7 runner docker 4096 Sep  2 11:46 .git
drwxr-xr-x 3 runner docker 4096 Sep  2 11:46 .github
drwxr-xr-x 2 runner docker 4096 Sep  2 11:46 .vscode
-rw-r--r-- 1 runner docker 1076 Sep  2 11:46 LICENSE
-rw-r--r-- 1 runner docker   53 Sep  2 11:46 README.md
drwxr-xr-x 2 runner docker 4096 Sep  2 11:46 casabot
-rw-r--r-- 1 runner docker  235 Sep  2 11:46 repository.yaml
```

### Casabot add-on directory
```
ls -la casabot/
total 28
drwxr-xr-x 2 runner docker 4096 Sep  2 11:46 .
drwxr-xr-x 6 runner docker 4096 Sep  2 11:46 ..
-rw-r--r-- 1 runner docker  314 Sep  2 11:46 Dockerfile
-rw-r--r-- 1 runner docker 1131 Sep  2 11:46 apparmor.txt
-rw-r--r-- 1 runner docker  679 Sep  2 11:46 build.yaml
-rw-r--r-- 1 runner docker  360 Sep  2 11:46 config.yaml
-rwxr-xr-x 1 runner docker   80 Sep  2 11:46 run.sh
```

### cat casabot/config.yaml
```yaml
name: "Casa Bot"
description: "An LLM enabled bot to interact with Home Assitant's MCP server"
version: "0.1.5"
slug: "casabot"
init: false
webui: https://[HOST]:[PORT:8000]
arch:
  - aarch64
  - amd64
  - armhf
  - armv7
  - i386
startup: services
ports:
  8000/tcp: 8000
ports_description:
  8000/tcp: "Web UI"
image: "ghcr.io/jmservera/{arch}-addon-casabot"
```

### cat casabot/run.sh
```bash
#!/usr/bin/with-contenv bashio

echo "Hello world!"

python3 -m http.server 8000
```

### cat casabot/Dockerfile
```dockerfile
ARG BUILD_FROM
FROM $BUILD_FROM

# Install requirements for add-on
RUN \
  apk add --no-cache \
    python3

# Python 3 HTTP Server serves the current working dir
# So let's set it to our add-on persistent data directory.
WORKDIR /data

# Copy data for add-on
COPY run.sh /
RUN chmod a+x /run.sh

CMD [ "/run.sh" ]
```

### Expected build times
- AMD64 build: ~8-10 seconds
- ARM builds (armv7, aarch64): ~8-10 seconds  
- Multi-arch build (all 5 architectures): ~45-60 seconds total

### Expected linting warnings (normal/expected)
- `shellcheck casabot/run.sh`: SC1008 warning about bashio shebang (expected for Home Assistant add-ons)
- `yamllint casabot/config.yaml`: Missing document start "---" and newline at end warnings

## Repository Structure
This is a single Home Assistant add-on repository with:
- **casabot/**: Main add-on directory containing all add-on files
- **.github/workflows/build.yaml**: GitHub Actions workflow for multi-arch builds using Home Assistant Builder
- **repository.yaml**: Repository metadata for Home Assistant add-on store
- **.vscode/copilot-instructions.md**: Legacy instructions (moved to .github/)

## Build System
- Uses official Home Assistant Builder for Docker image creation
- Supports 5 architectures: aarch64, amd64, armhf, armv7, i386
- Built images are tagged as `ghcr.io/jmservera/{arch}-addon-casabot`
- Build process handles cross-compilation automatically
- CI builds on push/PR to main branch using GitHub Actions

## Testing Requirements
- **Build Test**: Must build successfully for AMD64 and at least one ARM architecture
- **Functionality Test**: Container must start and serve HTTP on port 8000
- **HTTP Response Test**: Must return 200 status code for GET /
- **Content Test**: Must serve directory listing HTML content
- **Linting**: Pass yamllint and shellcheck (with expected warnings documented above)

## Security & Configuration
- Uses proper AppArmor profile in `casabot/apparmor.txt`
- Follows Home Assistant add-on security best practices
- No host networking or privileged access required
- Uses standard base images from Home Assistant
- Web UI exposed on port 8000 with proper port descriptions

## No Development Container
This repository does not include a `.devcontainer` setup. For local development:
- Use Docker directly with Home Assistant Builder
- Test builds locally before pushing changes
- Validate functionality by running built containers

## Common Error Patterns
- **Docker socket access**: Ensure `/var/run/docker.sock` is mapped when using Builder
- **Build timeouts**: ARM builds may take longer than AMD64 - use adequate timeouts
- **Image not found**: First build will pull base images, subsequent builds are faster
- **Port conflicts**: Ensure port 8000 is available when testing containers