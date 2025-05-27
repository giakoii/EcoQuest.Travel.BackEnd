using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Chatbot;

public class ChatbotRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "Question is required")]
    public required string Question { get; set; }
}