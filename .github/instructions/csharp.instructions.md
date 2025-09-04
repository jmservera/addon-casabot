---
applyTo: "casabot/src/**"
---
# 🤖 Copilot Instructions for app development

Welcome, Copilot! We’re building a **C# web application** with a **Blazor frontend** that acts as a chat UI.  
The project integrates with the **Home Assistant MCP server** using the [C# MCP SDK](https://github.com/modelcontextprotocol/csharp-sdk).  

## 🎯 Project Goals
- Provide a **modern chat interface** with:
  - Text input (textbox)
  - Audio input (microphone recording)
  - Chat history display
  - Audio + text responses
- Route **all chat interactions** (text in/out, audio in/out) through a **reusable API endpoint**.
- Ensure the solution can stream voice data directly from/to the endpoint.
- Keep the UI clean, minimal, and extensible.

## ✅ Copilot, please follow these guidelines:
1. **Structure**
   - Use **Blazor components** for UI.
   - Backend in **C#** with clear separation between frontend and API integration.
   - Follow best practices for dependency injection, async calls, and streaming.

2. **Integration**
   - Use the **C# MCP SDK** to connect with the Home Assistant MCP server.
   - Encapsulate MCP interactions in a service layer for reusability.

3. **Frontend**
   - Build a chat UI with:
     - Scrollable history area
     - Textbox input
     - Microphone button (for audio capture)
   - Support streaming partial responses (both text + audio).
   - Use relative paths and URLs for assets (no absolute URLs), ensuring compatibility with Home Assistant ingress.

4. **Backend**
   - API endpoint that handles:
     - Text requests/responses
     - Audio input (from browser mic)
     - Audio streaming output
   - Reusable so other clients (not only Blazor) can connect.

5. **Style**
   - Keep code **clean and documented**.
   - Use async/await properly.
   - Prefer interfaces and services for extensibility.
   - Handle errors gracefully (no silent fails).

## 🚀 Stretch Goals
- Enable persistent chat history.
- Add avatars or simple metadata in chat bubbles.
- Smooth audio playback with minimal delay.

---

💡 **Copilot, when in doubt:**
- Start by scaffolding the simplest working version.
- Use mock/dummy API calls if the backend isn’t ready.
- Suggest improvements iteratively.