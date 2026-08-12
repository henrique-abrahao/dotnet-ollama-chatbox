using System.Reflection;
using ChatAppAI.Services;
using Microsoft.Extensions.AI;
using Microsoft.OpenApi.Models;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ollama Chatbox API",
        Version = "v1",
        Description = "API de chatbot de IA usando Ollama + ASP.NET Core",
        Contact = new OpenApiContact
        {
            Name = "GitHub Repository",
            Url = new Uri("https://github.com/henrique-abrahao/dotnet-ollama-chatbox")
        }
    });

    // Include XML comments for documentation
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

IChatClient chatClient = new OllamaApiClient(
    new Uri("http://localhost:11434"),
    "llama3.2");

builder.Services.AddSingleton(chatClient);

builder.Services.AddSingleton<ConversationStore>();

builder.Services.AddScoped<IChatService, ChatService>();

var app = builder.Build();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ollama Chatbox API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root (http://localhost:5000/)
    });
}

app.MapControllers();

app.Run();