namespace ChatAppAI.Models;

public class ChatApiRequest
{
    public string Message { get; set; } = string.Empty;
    public string? ConversationId { get; set; }
}