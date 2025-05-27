using BackEnd.DTOs.Chatbot;
using BackEnd.Logics;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.ChatBotAI;

/// <summary>
/// ChatbotController - Handles chatbot requests
/// </summary>
//[ApiController]
//[Route("api/v1/[controller]")]
public class ChatbotController : AbstractApiAsyncControllerNotToken<ChatbotRequest, ChatbotResponse, ChatbotEntity>
{
    private readonly OpenAiLogic _openAiLogic;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="openAiLogic"></param>
    public ChatbotController(OpenAiLogic openAiLogic)
    {
        _openAiLogic = openAiLogic;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public override async Task<ChatbotResponse> ProcessRequest(ChatbotRequest request)
    {
        return await ProcessRequest(request, _logger, new ChatbotResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<ChatbotResponse> Exec(ChatbotRequest request)
    {
        var chatbotResponse = new ChatbotResponse { Success = false };

        //var chatbot = _openAiLogic.AskChatGpt();
        return null;
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override ChatbotResponse ErrorCheck(ChatbotRequest request, List<DetailError> detailErrorList)
    {
        var response = new ChatbotResponse() { Success = false };
        if (detailErrorList.Count > 0)
        {
            // Error
            response.SetMessage(MessageId.E10000);
            response.DetailErrorList = detailErrorList;
            return response;
        }
        // True
        response.Success = true;
        return response;
    }
}