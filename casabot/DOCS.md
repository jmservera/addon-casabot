# Casa Bot Add-on Documentation

## Installation

1. Navigate to **Supervisor** → **Add-on Store** in your Home Assistant interface
2. Click the menu (⋮) in the top right corner
3. Select **Repositories**
4. Add this repository URL: `https://github.com/jmservera/addon-casabot`
5. Find "Casa Bot" in the add-on store and click **Install**

## Configuration

Currently, the add-on works out of the box with minimal configuration required. Future versions will include configurable options for:

- LLM provider settings
- MCP server endpoints
- Chat behavior customization

### Example Configuration

```yaml
# Currently no options required
# Future options will be documented here
```

## Usage

### Access via Ingress (Recommended)

1. After starting the add-on, a new "Casa Bot" panel will appear in your Home Assistant sidebar
2. Click the panel to access the chat interface directly within Home Assistant
3. Start chatting with your smart home through natural language

### Direct Access

If you prefer direct access:

1. Navigate to `http://[HOME_ASSISTANT_IP]:8080`
2. The same chat interface will be available

## Architecture Details

### Service Management

The add-on uses **s6-overlay** for robust process supervision:

- **casabot service**: Runs the .NET Blazor application on port 8000
- **nginx service**: Reverse proxy handling requests on port 80

### Network Architecture

```
Home Assistant Ingress ──→ nginx (port 80) ──→ CasaBot (.NET app on port 8000)
              ↓
        Direct Access (port 8080)
```

### File Structure

- `/opt/casabot/`: CasaBot application files
- `/etc/nginx/`: nginx configuration
- `/etc/services.d/`: s6-overlay service definitions
- `/data/`: Persistent add-on data

## Security

The add-on implements several security best practices:

### Process Isolation

- **s6-overlay**: Proper process supervision and isolation
- **Non-root execution**: Services run with minimal privileges
- **AppArmor profile**: Restricted filesystem and network access

### Network Security

- **nginx reverse proxy**: Shields the application from direct exposure
- **Ingress integration**: Leverages Home Assistant's authentication
- **WebSocket security**: Proper upgrade handling for real-time features

### Permissions Required

The add-on requires these permissions:

- **Network access**: To communicate with MCP servers
- **Home Assistant API**: For smart home integrations (future feature)

## Troubleshooting

### Add-on won't start

1. Check the add-on logs for detailed error messages
2. Ensure you're running a supported Home Assistant version (2023.8+)
3. Verify system resources (minimum 512MB RAM recommended)

### Web UI not accessible

1. Verify the add-on is running (green status)
2. Check if port 8080 is accessible if using direct access
3. For ingress issues, try restarting Home Assistant

### Chat not responding

1. Check add-on logs for MCP connection errors
2. Verify Home Assistant's MCP server is running
3. Restart the add-on if needed

### Performance Issues

1. Monitor system resources in Home Assistant
2. Check nginx logs for request handling issues
3. Consider restarting the add-on periodically

## Advanced Configuration

### Custom nginx Configuration

Advanced users can modify nginx settings by:

1. Stopping the add-on
2. Editing configuration files in the add-on container
3. Restarting the add-on

**Note**: Custom configurations may be overwritten on add-on updates.

### Development Mode

For developers wanting to modify the CasaBot application:

1. Clone the repository
2. Use the included devcontainer for local development
3. Build custom images using the provided Dockerfile

## Changelog

### Version 0.1.6

- **NEW**: s6-overlay process management
- **NEW**: nginx reverse proxy integration
- **NEW**: Home Assistant ingress support
- **IMPROVED**: Enhanced security with AppArmor profiles
- **CHANGED**: Port configuration (now 8080 for direct access)
- **FIXED**: .NET 8 compatibility

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

Contributions are welcome! Please read our contributing guidelines and submit pull requests to the main repository.

## Support

- **Issues**: Report bugs and request features on [GitHub Issues](https://github.com/jmservera/addon-casabot/issues)
- **Community**: Join discussions in the Home Assistant Community Forum
- **Documentation**: Additional documentation available in the repository wiki
