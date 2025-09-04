# Changelog

All notable changes to CasaBot will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0] - 2024-12-XX

### Added
- Initial release of CasaBot
- Blazor-based chat interface with text and audio support
- Home Assistant MCP server integration
- Multi-architecture support (amd64, aarch64, armv7)
- Real-time chat messaging
- Audio recording and playback capabilities
- Bootstrap-based responsive UI
- Comprehensive service architecture with dependency injection

### Features
- **Chat Interface**: Modern, responsive chat UI with message history
- **Audio Support**: Browser-based microphone recording and audio playback
- **Home Assistant Integration**: Direct communication with Home Assistant APIs
- **MCP Protocol**: Support for Model Context Protocol for AI interactions
- **Multi-Platform**: Support for multiple CPU architectures

### Technical Details
- Built with .NET 8.0 and Blazor Server
- Uses SignalR for real-time communication
- Implements clean architecture with service separation
- Supports both development and production configurations