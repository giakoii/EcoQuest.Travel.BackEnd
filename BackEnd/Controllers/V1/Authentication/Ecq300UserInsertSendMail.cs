using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;

namespace BackEnd.Controllers.V1.Authentication;

/// <summary>
/// Ecq300UserInsertSendMail - Send mail to verify the user registration
/// </summary>
public static class Ecq300UserInsertSendMail
{
    /// <summary>
    /// Send mail to verify the user registration 
    /// </summary>
    /// <param name="emailTemplateRepository"></param>
    /// <param name="email"></param>
    /// <param name="key"></param>
    /// <param name="detailErrors"></param>
    /// <returns></returns>
    public static async Task<bool> SendMailVerifyInformation(IEmailTemplateRepository emailTemplateRepository, string email, string key, List<DetailError> detailErrors)
    {
        // Get the mail template
        var mailTemplate = await emailTemplateRepository.GetVerifyUserEmailTemplateAsync();
        var mailTitle = mailTemplate!.Title.Replace("${title}", mailTemplate.Title);
        
        var encodedKey = Uri.EscapeDataString(key);
        // Replace the variables in the mail template
        var replacements = new Dictionary<string, string>
        {
            { "${verification_link}", encodedKey }
        };
        
        // Replace the variables in the mail template
        var mailBody = mailTemplate.Body;
        foreach (var replacement in replacements)
        {
            mailBody = mailBody.Replace(replacement.Key, replacement.Value);
        }
        
        // Send the mail
        var mailInfo = new EmailTemplate
        {
            Title = mailTitle,
            Body = mailBody,
        };
        return await SendMailLogic.SendMail(mailInfo, email, emailTemplateRepository, detailErrors);
    }
}