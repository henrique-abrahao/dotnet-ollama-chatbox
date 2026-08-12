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

    [HttpPost]
    public async Task<IActionResult> Post(ChatApiRequest request)
    {
        var response = await _chatService.SendMessageAsync(request);

        return Ok(response);
    }

    [HttpPost("stream")]
    public async Task Stream(ChatApiRequest request)
    {
        Response.ContentType = "text/plain";

        await foreach (
            var chunk in _chatService.StreamMessageAsync(request))
        {
            await Response.WriteAsync(chunk);

            await Response.Body.FlushAsync();
        }
    }

    [HttpGet("{conversationId}")]
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