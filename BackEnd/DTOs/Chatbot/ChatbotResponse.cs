using BackEnd.Controllers;

namespace BackEnd.DTOs.Chatbot;

public class ChatbotResponse : AbstractApiResponse<ChatbotEntity>
{
    public override ChatbotEntity Response { get; set; }
}

public class ChatbotEntity
{
    public string Answer { get; set; }
}