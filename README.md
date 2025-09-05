# Casa Bot Home Assistant Add-on

A powerful LLM-enabled chatbot add-on that integrates with Home Assistant's MCP (Model Context Protocol) server, providing an intelligent interface to interact with your smart home.

## Features

- **Modern Blazor Web UI**: Clean, responsive chat interface
- **LLM Integration**: Powered by advanced language models via MCP
- **Home Assistant Integration**: Seamless access to your smart home data
- **Secure Architecture**: nginx reverse proxy with s6-overlay process management
- **Ingress Support**: Native Home Assistant panel integration
- **Real-time Communication**: WebSocket support for live interactions

## Quick Start

1. Add this repository to your Home Assistant add-on store
2. Install the "Casa Bot" add-on
3. Start the add-on
4. Access the web UI through the Home Assistant ingress panel or direct port

## Architecture

The add-on uses a modern containerized architecture with separated build process:

- **Package-based Build**: Uses pre-built CasaBot packages from the [casabot repository](https://github.com/jmservera/casabot)
- **s6-overlay**: Process supervision and service management
- **nginx**: Reverse proxy for secure and efficient request handling
- **CasaBot**: .NET 8 Blazor Server application (downloaded as pre-built package)
- **MCP Integration**: Direct communication with Home Assistant's MCP server

## Build Process

This add-on now uses a two-stage build process:

1. **CasaBot Application**: Built and packaged in the separate [casabot repository](https://github.com/jmservera/casabot)
2. **Add-on Container**: Downloads and integrates pre-built packages into the Home Assistant add-on

### Benefits

- **Faster Builds**: No need to compile .NET application during add-on build
- **Better Caching**: Pre-built packages reduce build times
- **Independent Development**: Application and add-on can be developed separately
- **Version Control**: Clear tracking of application versions

## Access Methods

- **Ingress Panel**: Integrated directly into Home Assistant (recommended)
- **Direct Access**: Available on port 8080 for advanced users

## Development

For application development, see the [casabot repository](https://github.com/jmservera/casabot).

For add-on development:

1. Clone this repository
2. Make changes to add-on configuration
3. Test with Home Assistant development environment

## Support

For issues, feature requests, and contributions, please visit:

- **Add-on Issues**: [addon-casabot repository](https://github.com/jmservera/addon-casabot)
- **Application Issues**: [casabot repository](https://github.com/jmservera/casabot)
