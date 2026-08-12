using ChatAppAI.Models;

namespace ChatAppAI.Services;

public class ConversationStore
{
    private readonly Dictionary<string, Conversation> _conversations = [];

    public Conversation GetOrCreate(string conversationId)
    {
        if (!_conversations.TryGetValue(conversationId, out var conversation))
        {
            conversation = new Conversation
            {
                Id = conversationId
            };

            _conversations[conversationId] = conversation;
        }

        return conversation;
    }

    public Conversation? Get(string conversationId)
    {
        _conversations.TryGetValue(
            conversationId,
            out var conversation);

        return conversation;
    }
}