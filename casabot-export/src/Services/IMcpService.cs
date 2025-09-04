namespace CasaBot.Services;

public interface IMcpService
{
    Task<string> SendMessageAsync(string message);
    Task InitializeAsync();
}