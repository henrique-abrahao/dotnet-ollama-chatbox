using ChatAppAI.Models;

namespace ChatAppAI.Services;

public interface IConversationStore
{
    Conversation GetOrCreate(string conversationId);
    Conversation? Get(string conversationId);
    bool Remove(string conversationId);
    IEnumerable<Conversation> GetAll();
}
