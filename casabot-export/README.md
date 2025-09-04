# CasaBot - Home Assistant Chat Interface

A modern chat interface for Home Assistant built with C# and Blazor, featuring both text and audio interactions.

## Features

- **Text Chat**: Clean, responsive chat interface with real-time messaging
- **Audio Support**: Voice input and audio response capabilities  
- **Home Assistant Integration**: Direct integration with Home Assistant APIs
- **Multi-Architecture**: Supports amd64, aarch64, and armv7 architectures

## Architecture

This application uses:
- **Backend**: C# with ASP.NET Core
- **Frontend**: Blazor Server components
- **Integration**: Home Assistant MCP (Model Context Protocol) server
- **Audio**: Browser-based audio recording and playback

## Development

### Prerequisites
- .NET 8.0 or later
- Home Assistant instance with MCP server configured

### Building
```bash
dotnet restore src/CasaBot.csproj
dotnet build src/CasaBot.csproj
dotnet run --project src/CasaBot.csproj
```

### Publishing
```bash
dotnet publish src/CasaBot.csproj \
  --configuration Release \
  --runtime linux-musl-x64 \
  --self-contained true \
  --output ./publish
```

## Deployment

This application is designed to run as a Home Assistant add-on. The add-on repository is at [jmservera/addon-casabot](https://github.com/jmservera/addon-casabot).

## Contributing

1. Fork this repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.