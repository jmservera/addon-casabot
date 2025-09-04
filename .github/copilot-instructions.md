# Casa Bot Home Assistant Add-on - Developer Instructions

**ALWAYS follow these instructions first** and only fallback to additional search and context gathering if the information here is incomplete or found to be in error.

Casa Bot is a Home Assistant add-on featuring a .NET 8 Blazor Server application that provides an LLM-enabled chat interface for interacting with Home Assistant's MCP (Model Context Protocol) server. The add-on uses s6-overlay for process supervision with nginx as a reverse proxy.

## Working Effectively

### Prerequisites and Bootstrap
- **.NET 8 SDK** is required and available (`dotnet --version` returns `8.0.119`)
- **Docker** is available for container builds
- **yamllint** is available for YAML validation
- **hadolint** is NOT available in this environment

### Build and Test Commands

#### .NET Application Build (NEVER CANCEL - 60+ second timeout recommended)
```bash
cd /home/runner/work/addon-casabot/addon-casabot/casabot/src
dotnet build
```
- **Build time**: ~17 seconds
- **Timeout recommendation**: Set 60+ seconds
- **Expected output**: Build succeeds with 1 warning about async method without await
- **Warning is expected**: `CS1998: This async method lacks 'await' operators` in Chat.razor line 255

#### Run Application for Testing (Development Mode)
```bash
cd /home/runner/work/addon-casabot/addon-casabot/casabot/src
dotnet run --no-build
```
- **Default port**: 5164 (configured in Properties/launchSettings.json)
- **Test access**: `curl -X GET http://localhost:5164` should return HTML content
- **Expected content**: HTML with title "Casa Bot Chat" and CasaBot navbar
- **Stop with**: Ctrl+C or `pkill -f CasaBot`

#### Docker Build (CRITICAL: Does NOT work in this environment)
```bash
cd /home/runner/work/addon-casabot/addon-casabot/casabot
docker build -t test-casabot --build-arg BUILD_ARCH=amd64 .
```
- **DOES NOT WORK**: Fails with NU1301 error (Unable to load NuGet service index)
- **Reason**: Network restrictions prevent access to api.nuget.org
- **Time to failure**: ~2 minutes
- **DO NOT ATTEMPT**: Docker builds will always fail due to firewall limitations

### Linting and Code Quality (NEVER CANCEL - 60+ second timeout recommended)

#### .NET Code Formatting
```bash
cd /home/runner/work/addon-casabot/addon-casabot/casabot/src
dotnet format --verify-no-changes --verbosity diagnostic
```
- **Time**: ~12 seconds
- **Timeout recommendation**: Set 60+ seconds
- **Exit code 2**: Indicates formatting issues found (expected)
- **Current issues**: Whitespace formatting errors in Program.cs, ChatService.cs, McpService.cs

#### Fix .NET Code Formatting
```bash
cd /home/runner/work/addon-casabot/addon-casabot/casabot/src
dotnet format
```
- **Time**: ~12 seconds
- **Use this**: To automatically fix formatting issues before committing

#### YAML Validation
```bash
cd /home/runner/work/addon-casabot/addon-casabot
yamllint casabot/config.yaml casabot/build.yaml .github/workflows/ci.yaml repository.yaml
```
- **Time**: ~0.1 seconds
- **Current issues**: Missing document starts, line length violations, missing newlines

## Repository Structure

### Key Directories and Files
```
/home/runner/work/addon-casabot/addon-casabot/
├── casabot/                          # Main add-on directory
│   ├── src/                          # .NET 8 Blazor application source
│   │   ├── CasaBot.csproj           # Project file
│   │   ├── Program.cs               # Application entry point
│   │   ├── Components/              # Blazor components
│   │   ├── Services/                # Application services
│   │   └── Properties/launchSettings.json  # Development settings
│   ├── rootfs/                      # Container filesystem overlay
│   │   └── etc/
│   │       ├── nginx/               # nginx configuration
│   │       └── s6-overlay/          # s6 service definitions
│   ├── config.yaml                  # Home Assistant add-on configuration
│   ├── build.yaml                   # Docker build configuration
│   ├── Dockerfile                   # Multi-stage container build
│   └── DOCS.md                      # Add-on documentation
├── .github/workflows/ci.yaml        # CI pipeline for multi-arch builds
├── repository.yaml                  # Repository metadata
└── README.md                        # Project overview
```

### Important Files to Check After Changes
- **Always run `dotnet format`** after editing .NET source files
- **Always run `yamllint`** after editing YAML configuration files
- **Check casabot/config.yaml** after modifying add-on configuration
- **Check .github/workflows/ci.yaml** after modifying CI pipeline

## Validation Scenarios

### Manual Validation Requirements
After making any changes, ALWAYS test the complete user workflow:

1. **Build the .NET application**:
   ```bash
   cd /home/runner/work/addon-casabot/addon-casabot/casabot/src
   dotnet build
   ```

2. **Run the application**:
   ```bash
   cd /home/runner/work/addon-casabot/addon-casabot/casabot/src
   dotnet run --no-build
   ```

3. **Test HTTP response** (in another terminal):
   ```bash
   curl -X GET http://localhost:5164
   ```
   - **Expected**: HTML response with "Casa Bot Chat" title
   - **Expected**: CasaBot navbar in response

4. **Stop the application**:
   ```bash
   pkill -f CasaBot
   ```

### Pre-commit Validation Checklist
Before committing changes, ALWAYS run:
```bash
# Format .NET code
cd /home/runner/work/addon-casabot/addon-casabot/casabot/src && dotnet format

# Validate YAML files
cd /home/runner/work/addon-casabot/addon-casabot && yamllint casabot/config.yaml casabot/build.yaml .github/workflows/ci.yaml repository.yaml

# Build application
cd /home/runner/work/addon-casabot/addon-casabot/casabot/src && dotnet build
```

## Architecture Details

### Process Architecture
- **s6-overlay**: Process supervision system
- **nginx**: Reverse proxy (port 80 internal, 8080 external)
- **CasaBot**: .NET 8 Blazor Server app (port 8000 internal)

### Home Assistant Integration
- **Ingress**: Native HA panel integration (`ingress: true`)
- **Auth API**: Uses HA authentication (`auth_api: true`)
- **Home Assistant API**: Access to HA APIs (`homeassistant_api: true`)
- **Direct Access**: Available on port 8080 for advanced users

### Docker Build Architecture
- **Multi-stage build**: Builder stage (.NET SDK) + Runtime stage (HA base)
- **Multi-arch support**: amd64, aarch64, armv7
- **Base images**: Uses `ghcr.io/home-assistant/{arch}-base:3.21`
- **Published images**: `ghcr.io/jmservera/{arch}-addon-casabot`

## CI Pipeline Details

### GitHub Actions Workflow
- **File**: `.github/workflows/ci.yaml`
- **Triggers**: Push, pull requests, manual dispatch
- **Builds**: Multi-architecture Docker images
- **Registry**: GitHub Container Registry (ghcr.io)
- **Linting**: Currently commented out (lint-addon, hadolint, etc.)

### Build Matrix
- **Architectures**: amd64, aarch64, armv7
- **Runners**: ARM builds use `ubuntu-24.04-arm`, others use `ubuntu-latest`
- **Cache**: Docker layer caching enabled

## Common Issues and Limitations

### Known Issues
1. **Docker builds fail** due to network restrictions (NU1301 NuGet error)
2. **Formatting issues** in .NET code (whitespace violations)
3. **YAML linting errors** in configuration files
4. **No unit tests** present in the codebase

### Network Limitations
- **NuGet access blocked**: Cannot restore packages in Docker builds
- **Workaround**: Use pre-built images or develop locally
- **CI builds work**: GitHub Actions has NuGet access

### Development Recommendations
- **Local development**: Use `dotnet run` for testing changes
- **Format code**: Always run `dotnet format` before committing
- **Validate YAML**: Use `yamllint` to check configuration files
- **Test manually**: Exercise the complete user workflow after changes

## Time Expectations

| Operation | Time | Timeout Setting |
|-----------|------|----------------|
| `dotnet build` | ~17 seconds | 60+ seconds |
| `dotnet format` | ~12 seconds | 60+ seconds |
| `dotnet run` startup | ~5 seconds | 30+ seconds |
| `yamllint` | ~0.1 seconds | 10 seconds |
| Docker build (fails) | ~2 minutes | Not applicable |

**CRITICAL**: NEVER CANCEL builds or long-running commands. Always set appropriate timeouts and wait for completion.