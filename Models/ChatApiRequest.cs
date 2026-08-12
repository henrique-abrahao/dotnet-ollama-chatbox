using System.ComponentModel.DataAnnotations;

namespace ChatAppAI.Models;

public class ChatApiRequest
{
    [Required(ErrorMessage = "A mensagem é obrigatória.")]
    [StringLength(4000, MinimumLength = 1, ErrorMessage = "A mensagem deve ter entre 1 e 4000 caracteres.")]
    public string Message { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "O ID da conversa não pode exceder 100 caracteres.")]
    public string? ConversationId { get; set; }
}