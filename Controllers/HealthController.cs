using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace ChatAppAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IChatClient _chatClient;

    public HealthController(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    /// <summary>
    /// Verifica o status de saúde da API e do serviço Ollama.
    /// </summary>
    /// <returns>Status da API e do Ollama</returns>
    /// <response code="200">API e Ollama estão saudáveis</response>
    /// <response code="503">Ollama está indisponível</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get()
    {
        var status = new
        {
            api = "Healthy",
            ollama = "Unknown",
            timestamp = DateTime.UtcNow
        };

        try
        {
            // Tenta fazer uma requisição simples ao Ollama para verificar disponibilidade
            // Usa um timeout curto para não bloquear a resposta
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            
            // Faz uma chamada mínima apenas para verificar conectividade
            var testMessages = new List<ChatMessage>
            {
                new(ChatRole.User, "test")
            };
            
            await _chatClient.GetResponseAsync(testMessages, cancellationToken: cts.Token);
            
            status = status with { ollama = "Healthy" };
            return Ok(status);
        }
        catch (OperationCanceledException)
        {
            status = status with { ollama = "Timeout" };
            return StatusCode(StatusCodes.Status503ServiceUnavailable, status);
        }
        catch (Exception)
        {
            status = status with { ollama = "Unavailable" };
            return StatusCode(StatusCodes.Status503ServiceUnavailable, status);
        }
    }
}
