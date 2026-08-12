using System.Reflection;
using ChatAppAI.Configuration;
using ChatAppAI.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
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