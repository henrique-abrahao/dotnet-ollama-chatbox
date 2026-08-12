using ChatAppAI.Services;
using Microsoft.Extensions.AI;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

IChatClient chatClient = new OllamaApiClient(
    new Uri("http://localhost:11434"),
    "llama3.2");

builder.Services.AddSingleton(chatClient);

builder.Services.AddSingleton<IConversationStore, ConversationStore>();

builder.Services.AddScoped<IChatService, ChatService>();

var app = builder.Build();

app.MapControllers();

app.Run();