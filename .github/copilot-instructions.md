# Casa Bot Home Assistant Add-on Development

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Repository Architecture

Casa Bot is a **Home Assistant add-on** that uses a **separated build architecture**:

- **This repository (addon-casabot)**: Contains Home Assistant add-on configuration and packaging
- **Application repository (casabot)**: Contains the .NET Blazor application source code and builds pre-built packages
- **Build Process**: Downloads pre-built CasaBot packages from GitHub releases rather than compiling from source

## Working Effectively

### Bootstrap and Validate the Repository

Always run these commands to validate the repository state:

```bash
# Validate YAML configuration files
yq eval '.' casabot/config.yaml > /dev/null && echo "✓ config.yaml is valid"
yq eval '.' casabot/build.yaml > /dev/null && echo "✓ build.yaml is valid"  
yq eval '.' .github/workflows/ci.yaml > /dev/null && echo "✓ ci.yaml is valid"
yq eval '.' repository.yaml > /dev/null && echo "✓ repository.yaml is valid"

# Run linting tools
shellcheck casabot/rootfs/etc/s6-overlay/s6-rc.d/casabot/run  # Check shell scripts
yamllint .  # Check YAML formatting (will show formatting issues - normal)

# Check for basic repository health
git status  # Ensure clean working tree
```

### Build the Add-on

**NEVER CANCEL** build commands - they complete quickly (4-5 seconds typical).

```bash
cd casabot

# Build for amd64 (default architecture)
docker build \
  --build-arg BUILD_FROM=ghcr.io/home-assistant/amd64-base:3.21 \
  --build-arg BUILD_ARCH=amd64 \
  --build-arg CASABOT_VERSION=0.2.0 \
  --build-arg BUILD_DATE=$(date +"%Y-%m-%dT%H:%M:%SZ") \
  --build-arg BUILD_DESCRIPTION="Casa Bot add-on" \
  --build-arg BUILD_NAME="Casa Bot" \
  --build-arg BUILD_REF=$(git rev-parse HEAD) \
  --build-arg BUILD_REPOSITORY=jmservera/addon-casabot \
  --build-arg BUILD_VERSION=0.2.0 \
  -t casabot-test .

# Build time: ~4 seconds cached, ~4 seconds clean build
# Timeout: Set to 60+ seconds minimum for safety
```

### Validate Build Results

After building, always validate the container:

```bash
# Check image was created
docker images casabot-test

# Verify CasaBot files are present
docker run --rm --entrypoint=/bin/sh casabot-test -c "ls -la /opt/casabot/ | head -5"

# Test container startup (will fail outside Home Assistant - expected)
docker run --rm casabot-test  # Should show s6-overlay startup and CasaBot launch
```

### Test Package Downloads

Verify the CasaBot package download mechanism works:

```bash
# Test current version download
curl -s -L "https://github.com/jmservera/casabot/releases/download/v0.2.0/casabot-amd64.tar.gz" --head | head -5

# Verify package contents (in /tmp to avoid repository clutter)
mkdir -p /tmp/test_download
cd /tmp/test_download
curl -s -L "https://github.com/jmservera/casabot/releases/download/v0.2.0/casabot-amd64.tar.gz" -o casabot.tar.gz
tar -tzf casabot.tar.gz | grep "CasaBot$"  # Should show ./CasaBot
rm -rf /tmp/test_download
```

## Development Workflow

### Configuration Changes

When modifying add-on configuration:

1. **Edit configuration files**:
   - `casabot/config.yaml` - Home Assistant add-on configuration
   - `casabot/build.yaml` - Docker build configuration  
   - `casabot/Dockerfile` - Container build instructions

2. **Validate changes**:
   ```bash
   yq eval '.' casabot/config.yaml  # Validate YAML syntax
   yq eval '.args.CASABOT_VERSION' casabot/build.yaml  # Check version
   ```

3. **Test build**:
   ```bash
   cd casabot && docker build --build-arg BUILD_FROM=ghcr.io/home-assistant/amd64-base:3.21 --build-arg BUILD_ARCH=amd64 --build-arg CASABOT_VERSION=$(yq eval '.args.CASABOT_VERSION' build.yaml) -t casabot-test .
   ```

### Service Configuration Changes

When modifying s6-overlay services or nginx configuration:

1. **Key files**:
   - `casabot/rootfs/etc/s6-overlay/s6-rc.d/casabot/run` - CasaBot service startup
   - `casabot/rootfs/etc/nginx/nginx.conf` - nginx configuration
   - `casabot/rootfs/etc/nginx/includes/` - nginx include files

2. **Validate shell scripts**:
   ```bash
   shellcheck casabot/rootfs/etc/s6-overlay/s6-rc.d/casabot/run
   shellcheck casabot/rootfs/etc/s6-overlay/s6-rc.d/init-*/run
   ```

3. **Always build and test** after service changes

### Version Updates

To update the CasaBot application version:

1. **Update version references**:
   ```bash
   # Check current version
   yq eval '.args.CASABOT_VERSION' casabot/build.yaml
   
   # Update build.yaml
   yq eval '.args.CASABOT_VERSION = "NEW_VERSION"' -i casabot/build.yaml
   
   # Update config.yaml if needed
   yq eval '.version' casabot/config.yaml
   ```

2. **Test new version download**:
   ```bash
   NEW_VERSION=$(yq eval '.args.CASABOT_VERSION' casabot/build.yaml)
   curl -s -L "https://github.com/jmservera/casabot/releases/download/v${NEW_VERSION}/casabot-amd64.tar.gz" --head | head -1
   ```

3. **Build and validate**

## Validation Checklist

Always run this complete validation sequence before committing changes:

```bash
# 1. YAML validation (2-3 seconds)
echo "=== YAML Validation ==="
yq eval '.' casabot/config.yaml > /dev/null && echo "✓ config.yaml valid" || echo "✗ config.yaml invalid"
yq eval '.' casabot/build.yaml > /dev/null && echo "✓ build.yaml valid" || echo "✗ build.yaml invalid"
yq eval '.' .github/workflows/ci.yaml > /dev/null && echo "✓ ci.yaml valid" || echo "✗ ci.yaml invalid"
yq eval '.' repository.yaml > /dev/null && echo "✓ repository.yaml valid" || echo "✗ repository.yaml invalid"

# 2. Shell script linting (1-2 seconds)
echo "=== Shell Script Linting ==="
shellcheck casabot/rootfs/etc/s6-overlay/s6-rc.d/casabot/run

# 3. Docker build test (4-5 seconds) - NEVER CANCEL
echo "=== Docker Build Test ==="
cd casabot && time docker build \
  --build-arg BUILD_FROM=ghcr.io/home-assistant/amd64-base:3.21 \
  --build-arg BUILD_ARCH=amd64 \
  --build-arg CASABOT_VERSION=$(yq eval '.args.CASABOT_VERSION' build.yaml) \
  --build-arg BUILD_DATE=$(date +"%Y-%m-%dT%H:%M:%SZ") \
  -t casabot-validation-test . && echo "✓ Build successful" || echo "✗ Build failed"

# 4. Container validation (1 second)
echo "=== Container Validation ==="
docker run --rm --entrypoint=/bin/sh casabot-validation-test -c "ls -la /opt/casabot/CasaBot" && echo "✓ CasaBot executable present"

# 5. Cleanup
docker rmi casabot-validation-test
```

**Total validation time: ~10 seconds**

## Timing Expectations

- **YAML validation**: <1 second each file
- **shellcheck**: <1 second  
- **yamllint**: <2 seconds (will show formatting warnings - normal)
- **Docker build (cached)**: ~4 seconds
- **Docker build (clean)**: ~4 seconds  
- **Container validation**: <1 second
- **Package download test**: 1-2 seconds

**NEVER CANCEL** any build command. All operations complete quickly.

## Manual Testing

Since this is a Home Assistant add-on, full functionality testing requires a Home Assistant environment. However, you can validate:

1. **Container starts correctly** (will fail due to missing supervisor - expected)
2. **CasaBot executable is present and has correct permissions**
3. **nginx configuration is valid**
4. **s6-overlay services are properly configured**

## Common Tasks and File Locations

### Repository Root
```
.github/                 # GitHub Actions workflows and instructions
├── workflows/ci.yaml   # Main CI/CD pipeline
├── copilot-instructions.md
└── instructions/
casabot/                # Home Assistant add-on files
├── config.yaml         # Add-on configuration
├── build.yaml          # Docker build configuration  
├── Dockerfile          # Container build instructions
├── DOCS.md            # User documentation
└── rootfs/            # Container runtime files
    ├── etc/nginx/     # nginx configuration
    └── etc/s6-overlay/ # s6-overlay service definitions
README.md              # Repository overview
DOCS.md               # Build process documentation  
repository.yaml       # Home Assistant repository metadata
```

### Key Configuration Files
- **`casabot/config.yaml`**: Home Assistant add-on metadata, options, schema, permissions
- **`casabot/build.yaml`**: Docker build arguments including CASABOT_VERSION
- **`casabot/Dockerfile`**: Downloads pre-built packages, installs runtime dependencies
- **`casabot/rootfs/etc/s6-overlay/s6-rc.d/casabot/run`**: CasaBot service startup script

### Architecture Support
Supports 3 architectures: **amd64**, **aarch64**, **armv7**

All builds download architecture-specific packages:
- `casabot-amd64.tar.gz`  
- `casabot-aarch64.tar.gz`
- `casabot-armv7.tar.gz`

## Troubleshooting

### Build Failures
1. **Package not found**: Check if CASABOT_VERSION exists in GitHub releases
2. **Base image issues**: Ensure Home Assistant base image is available
3. **Download failures**: Verify GitHub releases are accessible

### Shell Script Issues
- Always fix shellcheck warnings before committing
- Common issue: Use `cd /path || exit` instead of just `cd /path`

### YAML Issues
- Use `---` document separator at start of YAML files
- Maintain 80-character line limits
- Add newlines at end of files

## CI/CD Integration

The `.github/workflows/ci.yaml` handles:
- Multi-architecture builds (amd64, aarch64, armv7)
- Automatic version extraction from `build.yaml`
- Container registry publishing to `ghcr.io/jmservera/{arch}-addon-casabot`

Most linters are commented out in CI but available locally for development validation.

## Do NOT

- **Build .NET application from source** - this repository downloads pre-built packages
- **Modify the .sln file** - it's obsolete from the previous monolithic architecture  
- **Cancel Docker builds** - they complete in seconds, not minutes
- **Try to run Home Assistant specific functionality outside HA** - will fail due to missing supervisor API

## Always

- **Validate YAML files with yq** before committing
- **Run shellcheck on shell scripts** before committing  
- **Test Docker builds** after any Dockerfile or configuration changes
- **Check package download URLs** when updating CASABOT_VERSION
- **Follow the complete validation checklist** before finalizing changes