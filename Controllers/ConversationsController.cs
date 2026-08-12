using ChatAppAI.Models;
using ChatAppAI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationsController : ControllerBase
{
    private readonly IConversationStore _conversationStore;

    public ConversationsController(IConversationStore conversationStore)
    {
        _conversationStore = conversationStore;
    }

    /// <summary>
    /// Lista todas as conversas armazenadas em memória.
    /// </summary>
    /// <returns>Lista de conversas</returns>
    /// <response code="200">Lista de conversas retornada com sucesso</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Conversation>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var conversations = _conversationStore.GetAll();
        return Ok(conversations);
    }

    /// <summary>
    /// Recupera uma conversa específica pelo ID.
    /// </summary>
    /// <param name="id">ID da conversa</param>
    /// <returns>Conversa completa com histórico de mensagens</returns>
    /// <response code="200">Conversa encontrada</response>
    /// <response code="404">Conversa não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Conversation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(string id)
    {
        var conversation = _conversationStore.Get(id);

        if (conversation is null)
        {
            return NotFound(new { message = $"Conversa '{id}' não encontrada." });
        }

        return Ok(conversation);
    }

    /// <summary>
    /// Deleta uma conversa da memória.
    /// </summary>
    /// <param name="id">ID da conversa a ser deletada</param>
    /// <response code="204">Conversa deletada com sucesso</response>
    /// <response code="404">Conversa não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(string id)
    {
        var removed = _conversationStore.Remove(id);

        if (!removed)
        {
            return NotFound(new { message = $"Conversa '{id}' não encontrada." });
        }

        return NoContent();
    }
}
