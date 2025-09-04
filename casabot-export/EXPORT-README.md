# CasaBot Repository Export Package

This folder contains a complete export package for creating the separate `jmservera/casabot` repository. All source code and configuration files have been prepared and are ready to be used to create the new repository.

## Contents

This export package includes:

### Source Code (`src/`)
- **27 source files** - Complete C# Blazor application
- **CasaBot.csproj** - Project configuration
- **Components/** - Blazor UI components (App, Layout, Pages)  
- **Models/** - Data models for chat functionality
- **Services/** - Business logic and MCP integration
- **wwwroot/** - Static web assets (CSS, JS, images)
- **Configuration files** - appsettings.json, launchSettings.json

### Repository Configuration
- **.github/workflows/build.yml** - Multi-architecture CI/CD pipeline
- **.gitignore** - Standard .NET gitignore
- **README.md** - Application documentation
- **CHANGELOG.md** - Version history
- **LICENSE** - MIT license
- **EXPORT-README.md** - This file

## Creating the New Repository

### Option 1: Manual Creation (Recommended)

1. **Create new GitHub repository**:
   - Go to https://github.com/new
   - Repository name: `casabot`
   - Owner: `jmservera`
   - Description: "Home Assistant Chat Interface - C# Blazor Application"
   - Make it public
   - Don't initialize with README, .gitignore, or license (we have them)

2. **Upload the files**:
   - Clone the new empty repository locally
   - Copy all files from this `casabot-export/` folder to the repository root
   - Maintain the directory structure exactly as shown

3. **Initialize repository**:
   ```bash
   cd /path/to/casabot
   git add .
   git commit -m "Initial commit: CasaBot v0.1.0"
   git push origin main
   ```

4. **Create first release**:
   - Go to repository → Releases → Create a new release
   - Tag version: `v0.1.0`
   - Release title: `CasaBot v0.1.0`
   - Description: "Initial release of CasaBot application"
   - Publish release
   - This will trigger the GitHub Actions workflow to build packages

### Option 2: GitHub CLI (if you have it installed)

```bash
# Create repository
gh repo create jmservera/casabot --public --description "Home Assistant Chat Interface - C# Blazor Application"

# Clone and setup
git clone https://github.com/jmservera/casabot.git
cd casabot

# Copy all files from casabot-export/ to current directory
# (maintain directory structure)

# Commit and push
git add .
git commit -m "Initial commit: CasaBot v0.1.0"
git push origin main

# Create release
gh release create v0.1.0 --title "CasaBot v0.1.0" --notes "Initial release of CasaBot application"
```

## Verification Steps

After creating the repository:

1. **Verify structure**:
   ```
   casabot/
   ├── .github/workflows/build.yml
   ├── src/
   │   ├── Components/
   │   ├── Models/
   │   ├── Services/
   │   ├── wwwroot/
   │   └── CasaBot.csproj
   ├── .gitignore
   ├── README.md
   ├── CHANGELOG.md
   └── LICENSE
   ```

2. **Test GitHub Actions**:
   - Check that the workflow runs successfully
   - Verify packages are created for all architectures (amd64, aarch64, armv7)
   - Confirm packages are attached to the release

3. **Test local build**:
   ```bash
   dotnet restore src/CasaBot.csproj
   dotnet build src/CasaBot.csproj
   ```

## Integration with Add-on

Once the casabot repository is created and the first release is published:

1. **Update add-on Dockerfile** (already done in addon-casabot):
   - Downloads packages from casabot releases
   - Supports version specification via `CASABOT_VERSION` arg
   - Falls back to latest release if version not specified

2. **Test add-on build**:
   - Build the addon-casabot with new architecture
   - Verify it downloads and installs casabot packages correctly

## File Count Summary

- **Total files**: 32
- **Source code files**: 27
- **Configuration files**: 5 (.github/workflows/build.yml, .gitignore, README.md, CHANGELOG.md, LICENSE)

All files have been successfully extracted from the git history before they were removed and are ready for use in the new repository.

## Next Steps

1. Create the casabot repository using one of the methods above
2. Verify the first release builds successfully  
3. Test the addon-casabot build with the new package-based architecture
4. Update documentation as needed

The package-based architecture will enable:
- **Faster add-on builds** (no 5+ minute .NET compilation)
- **Independent development** of application vs. add-on
- **Better version management** with separate release cycles
- **Improved caching** of pre-built packages