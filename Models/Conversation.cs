using Microsoft.Extensions.AI;

namespace ChatAppAI.Models;

public class Conversation
{
    public string Id { get; set; } = string.Empty;

    public List<ChatMessage> Messages { get; set; } = [];
}