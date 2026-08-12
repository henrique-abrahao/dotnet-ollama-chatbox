using ChatAppAI.Configuration;
using ChatAppAI.Middleware;
using ChatAppAI.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add ProblemDetails for standard error responses
builder.Services.AddProblemDetails();

// Configure Ollama options from appsettings.json
builder.Services.Configure<OllamaOptions>(
    builder.Configuration.GetSection("Ollama"));

// Register IChatClient using configured options
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
    return new OllamaApiClient(new Uri(options.BaseUrl), options.Model);
});

builder.Services.AddSingleton<IConversationStore, ConversationStore>();

builder.Services.AddScoped<IChatService, ChatService>();

var app = builder.Build();

// Global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();