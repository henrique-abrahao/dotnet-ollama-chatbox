using ChatAppAI.Models;

namespace ChatAppAI.Services;

public interface IChatService
{
    Task<ChatApiResponse> SendMessageAsync(ChatApiRequest request);
    IAsyncEnumerable<string> StreamMessageAsync(ChatApiRequest request);
    Conversation? GetConversation(string conversationId);
}