using System.Collections.Concurrent;
using ChatAppAI.Models;

namespace ChatAppAI.Services;

public class ConversationStore : IConversationStore
{
    private readonly ConcurrentDictionary<string, Conversation> _conversations = new();

    public Conversation GetOrCreate(string conversationId)
    {
        return _conversations.GetOrAdd(conversationId, key => new Conversation
        {
            Id = key
        });
    }

    public Conversation? Get(string conversationId)
    {
        _conversations.TryGetValue(conversationId, out var conversation);
        return conversation;
    }

    public bool Remove(string conversationId)
    {
        return _conversations.TryRemove(conversationId, out _);
    }

    public IEnumerable<Conversation> GetAll()
    {
        return _conversations.Values;
    }
}