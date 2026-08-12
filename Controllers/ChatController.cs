using ChatAppAI.Models;
using ChatAppAI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Envia uma mensagem ao chatbot e recebe a resposta completa.
    /// </summary>
    /// <param name="request">Requisição contendo a mensagem e opcionalmente o ID da conversa</param>
    /// <returns>Resposta do chatbot com o ID da conversa</returns>
    /// <response code="200">Resposta gerada com sucesso</response>
    /// <response code="400">Requisição inválida (validação falhou)</response>
    [HttpPost]
    [ProducesResponseType(typeof(ChatApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] ChatApiRequest request, CancellationToken cancellationToken)
    {
        var response = await _chatService.SendMessageAsync(request, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Envia uma mensagem ao chatbot e recebe a resposta em streaming.
    /// </summary>
    /// <param name="request">Requisição contendo a mensagem e opcionalmente o ID da conversa</param>
    /// <response code="200">Stream de resposta iniciado</response>
    /// <response code="400">Requisição inválida (validação falhou)</response>
    [HttpPost("stream")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task Stream(
        [FromBody] ChatApiRequest request,
        CancellationToken cancellationToken)
    {
        Response.ContentType = "text/plain";

        await foreach (
            var chunk in _chatService.StreamMessageAsync(request, cancellationToken))
        {
            await Response.WriteAsync(chunk, cancellationToken);

            await Response.Body.FlushAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Recupera o histórico completo de uma conversa pelo seu ID.
    /// </summary>
    /// <param name="conversationId">ID da conversa</param>
    /// <returns>Histórico completo da conversa</returns>
    /// <response code="200">Conversa encontrada</response>
    /// <response code="404">Conversa não encontrada</response>
    [HttpGet("{conversationId}")]
    [ProducesResponseType(typeof(Conversation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetConversation(string conversationId)
    {
        var conversation =
            _chatService.GetConversation(conversationId);

        if (conversation is null)
        {
            return NotFound();
        }

        return Ok(conversation);
    }
}