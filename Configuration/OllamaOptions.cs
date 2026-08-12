namespace ChatAppAI.Configuration;

public class OllamaOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "llama3.2";
    public string SystemPrompt { get; set; } = string.Empty;
}
