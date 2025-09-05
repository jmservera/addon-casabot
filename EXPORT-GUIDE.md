# Files to Export to CasaBot Repository

This document lists all files that need to be exported from the `addon-casabot` repository to create the new `casabot` repository.

## Directory Structure

The new `casabot` repository should have the following structure:

```
casabot/
├── .github/
│   └── workflows/
│       └── build.yml
├── src/
│   ├── Components/
│   │   ├── App.razor
│   │   ├── Layout/
│   │   │   ├── MainLayout.razor
│   │   │   ├── MainLayout.razor.css
│   │   │   ├── NavMenu.razor
│   │   │   └── NavMenu.razor.css
│   │   └── Pages/
│   │       ├── Chat.razor
│   │       ├── Counter.razor
│   │       ├── Error.razor
│   │       ├── Home.razor
│   │       └── Weather.razor
│   ├── Models/
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Services/
│   ├── wwwroot/
│   │   ├── app.css
│   │   ├── bootstrap/
│   │   └── favicon.png
│   ├── CasaBot.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── .gitignore
├── README.md
├── CHANGELOG.md
└── LICENSE
```

## Files to Copy from addon-casabot

### Source Code (from casabot/src/)

All files in the `casabot/src/` directory should be copied to `src/` in the new repository:

- `CasaBot.csproj` - Project file
- `Program.cs` - Application entry point
- `appsettings.json` - Production configuration
- `appsettings.Development.json` - Development configuration
- `Components/` - All Blazor components
- `Models/` - Data models
- `Properties/` - Project properties
- `Services/` - Application services
- `wwwroot/` - Static web assets

### License and Documentation

- `LICENSE` - MIT license file
- Create new `README.md` - Application-specific documentation
- Create new `CHANGELOG.md` - Application version history

### Build Configuration

- Create `.github/workflows/build.yml` - Multi-architecture build pipeline
- Create `.gitignore` - Standard .NET gitignore

## Files to Create in CasaBot Repository

### New GitHub Actions Workflow (.github/workflows/build.yml)

```yaml
name: Build and Release

on:
  push:
    branches: [main]
    tags: ["v*"]
  pull_request:
    branches: [main]
  workflow_dispatch:

env:
  DOTNET_VERSION: "8.0.x"
  BUILD_CONFIGURATION: Release

jobs:
  build:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        arch: [amd64, aarch64, armv7]
        include:
          - arch: amd64
            runtime: linux-musl-x64
          - arch: aarch64
            runtime: linux-musl-arm64
          - arch: armv7
            runtime: linux-musl-arm

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Restore dependencies
        run: dotnet restore src/CasaBot.csproj

      - name: Build
        run: dotnet build src/CasaBot.csproj --no-restore --configuration ${{ env.BUILD_CONFIGURATION }}

      - name: Publish
        run: |
          dotnet publish src/CasaBot.csproj \
            --configuration ${{ env.BUILD_CONFIGURATION }} \
            --runtime ${{ matrix.runtime }} \
            --self-contained true \
            --output ./publish/${{ matrix.arch }}

      - name: Create package
        run: |
          cd ./publish/${{ matrix.arch }}
          tar -czf ../casabot-${{ matrix.arch }}.tar.gz .
          cd ..
          sha256sum casabot-${{ matrix.arch }}.tar.gz > casabot-${{ matrix.arch }}.tar.gz.sha256

      - name: Upload build artifacts
        uses: actions/upload-artifact@v4
        with:
          name: casabot-${{ matrix.arch }}
          path: |
            ./publish/casabot-${{ matrix.arch }}.tar.gz
            ./publish/casabot-${{ matrix.arch }}.tar.gz.sha256

  release:
    if: startsWith(github.ref, 'refs/tags/v')
    needs: build
    runs-on: ubuntu-latest
    permissions:
      contents: write

    steps:
      - uses: actions/checkout@v4

      - name: Download all artifacts
        uses: actions/download-artifact@v4
        with:
          path: ./artifacts

      - name: Prepare release assets
        run: |
          mkdir -p ./release-assets
          find ./artifacts -name "*.tar.gz*" -exec cp {} ./release-assets/ \;

      - name: Create Release
        uses: softprops/action-gh-release@v2
        with:
          files: ./release-assets/*
          draft: false
          prerelease: false
          generate_release_notes: true
```

### .gitignore

```
bin/
obj/
publish/
*.user
*.suo
*.cache
*.log
.vs/
.vscode/
```

## Repository Setup Steps

1. Create new GitHub repository `jmservera/casabot`
2. Copy all files from the list above
3. Set up repository settings:
   - Enable GitHub Actions
   - Configure branch protection for main branch
   - Add repository description and topics
4. Create initial release tag (v0.1.0) to trigger first package build
5. Verify packages are built and published to GitHub Releases

## Integration with Add-on Repository

After creating the casabot repository:

1. Remove `casabot/src/` directory from addon-casabot
2. Update `casabot/Dockerfile` to download packages from new repository
3. Test add-on build with new architecture
4. Update documentation in addon-casabot repository

This separation allows for independent development and versioning of the application while maintaining the add-on packaging and configuration in the original repository.
