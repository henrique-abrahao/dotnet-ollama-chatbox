using System.Runtime.CompilerServices;
using System.Text;
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

    public async Task<ChatApiResponse> SendMessageAsync(ChatApiRequest request, CancellationToken cancellationToken = default)
    {
        var (conversationId, conversation) = PrepareConversation(request);

        var response = await _chatClient.GetResponseAsync(
            conversation.Messages,
            cancellationToken: cancellationToken
        );

        conversation.Messages.Add(
            new ChatMessage(ChatRole.Assistant, response.Text)
        );

        return new ChatApiResponse
        {
            ConversationId = conversationId,
            Message = response.Text
        };
    }

    public async IAsyncEnumerable<string> StreamMessageAsync(
        ChatApiRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var (conversationId, conversation) = PrepareConversation(request);

        var fullResponse = new StringBuilder();

        await foreach (
            var update in _chatClient.GetStreamingResponseAsync(
                conversation.Messages,
                cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullResponse.Append(update.Text);
                yield return update.Text;
            }
        }

        conversation.Messages.Add(
            new ChatMessage(ChatRole.Assistant, fullResponse.ToString())
        );
    }

    private (string ConversationId, Conversation Conversation) PrepareConversation(
        ChatApiRequest request)
    {
        var conversationId = string.IsNullOrWhiteSpace(request.ConversationId)
            ? Guid.NewGuid().ToString()
            : request.ConversationId;

        var conversation = _conversationStore.GetOrCreate(conversationId);

        // Add system prompt only for new conversations
        if (conversation.Messages.Count == 0 && !string.IsNullOrWhiteSpace(_systemPrompt))
        {
            conversation.Messages.Add(
                new ChatMessage(ChatRole.System, _systemPrompt)
            );
        }

        // Add user message
        conversation.Messages.Add(
            new ChatMessage(ChatRole.User, request.Message)
        );

        return (conversationId, conversation);
    }
}