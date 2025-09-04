using CasaBot.Models;

namespace CasaBot.Services;

public interface IChatService
{
    Task<ChatResponse> SendMessageAsync(string message);
    Task<ChatResponse> SendAudioAsync(string audioData);
}