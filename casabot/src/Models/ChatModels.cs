namespace CasaBot.Models;

public class ChatMessage
{
    public string Content { get; set; } = string.Empty;
    public bool IsUser { get; set; }
    public DateTime Timestamp { get; set; }
    public string? AudioUrl { get; set; }
}

public class ChatResponse
{
    public string Text { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
}