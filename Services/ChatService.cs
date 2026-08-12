using ChatAppAI.Configuration;
using ChatAppAI.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace ChatAppAI.Services;

public class ChatService : IChatService
{
    private readonly IChatClient _chatClient;
    private readonly ConversationStore _conversationStore;
    private readonly string _systemPrompt;

    public ChatService(
        IChatClient chatClient,
        ConversationStore conversationStore,
        IOptions<OllamaOptions> options)
    {
        _chatClient = chatClient;
        _conversationStore = conversationStore;
        _systemPrompt = options.Value.SystemPrompt;
    }

    public Conversation? GetConversation(string conversationId)
    {
        return _conversationStore.Get(conversationId);
    }

    public async Task<ChatApiResponse> SendMessageAsync(ChatApiRequest request)
    {
        var conversationId = string.IsNullOrWhiteSpace(request.ConversationId)
            ? Guid.NewGuid().ToString()
            : request.ConversationId;

        var conversation =
            _conversationStore.GetOrCreate(conversationId);

        if (conversation.Messages.Count == 0 && !string.IsNullOrWhiteSpace(_systemPrompt))
        {
            conversation.Messages.Add(
                new ChatMessage(ChatRole.System, _systemPrompt)
            );
        }

        conversation.Messages.Add(
            new ChatMessage(
                ChatRole.User,
                request.Message
            )
        );

        var response =
            await _chatClient.GetResponseAsync(
                conversation.Messages
            );

        conversation.Messages.Add(
            new ChatMessage(
                ChatRole.Assistant,
                response.Text
            )
        );

        return new ChatApiResponse
        {
            ConversationId = conversationId,
            Message = response.Text
        };
    }

    public async IAsyncEnumerable<string> StreamMessageAsync(
    ChatApiRequest request)
    {
        var conversationId = string.IsNullOrWhiteSpace(request.ConversationId)
            ? Guid.NewGuid().ToString()
            : request.ConversationId;

        var conversation =
            _conversationStore.GetOrCreate(conversationId);

        if (conversation.Messages.Count == 0 && !string.IsNullOrWhiteSpace(_systemPrompt))
        {
            conversation.Messages.Add(
                new ChatMessage(ChatRole.System, _systemPrompt)
            );
        }

        conversation.Messages.Add(
            new ChatMessage(
                ChatRole.User,
                request.Message
            )
        );

        var fullResponse = "";

        await foreach (
            var update in _chatClient.GetStreamingResponseAsync(
                conversation.Messages))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullResponse += update.Text;

                yield return update.Text;
            }
        }

        conversation.Messages.Add(
            new ChatMessage(
                ChatRole.Assistant,
                fullResponse
            )
        );
    }
}