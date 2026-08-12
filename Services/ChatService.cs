using ChatAppAI.Models;
using Microsoft.Extensions.AI;

namespace ChatAppAI.Services;

public class ChatService : IChatService
{
    private readonly IChatClient _chatClient;
    private readonly ConversationStore _conversationStore;

    public ChatService(
        IChatClient chatClient,
        ConversationStore conversationStore)
    {
        _chatClient = chatClient;
        _conversationStore = conversationStore;
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

        if (conversation.Messages.Count == 0)
        {
            conversation.Messages.Add(
                new ChatMessage(ChatRole.System, """
                    You are a friendly hiking enthusiast who helps people discover fun hikes in their area.

                    You introduce yourself when first saying hello.

                    When helping people out, you always ask them for this information:

                    1. The location where they would like to hike
                    2. What hiking intensity they are looking for

                    You will then provide three suggestions for nearby hikes that vary in length.

                    You will also share an interesting fact about the local nature
                    when making a recommendation.

                    At the end of your response, ask if there is anything else you can help with.
                    """)
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

        if (conversation.Messages.Count == 0)
        {
            conversation.Messages.Add(
                new ChatMessage(ChatRole.System, """
                You are a friendly hiking enthusiast who helps people discover fun hikes in their area.

                You introduce yourself when first saying hello.

                When helping people out, you always ask them for this information:

                1. The location where they would like to hike
                2. What hiking intensity they are looking for

                You will then provide three suggestions for nearby hikes that vary in length.

                You will also share an interesting fact about the local nature
                when making a recommendation.

                At the end of your response, ask if there is anything else you can help with.
                """)
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