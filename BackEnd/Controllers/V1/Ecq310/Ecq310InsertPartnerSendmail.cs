using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;

namespace BackEnd.Controllers.V1.Ecq310;

public static class Ecq310InsertPartnerSendmail
{
    /// <summary>
    /// Send mail to verify the user registration 
    /// </summary>
    /// <param name="emailTemplateRepository"></param>
    /// <param name="email"></param>
    /// <param name="key"></param>
    /// <param name="detailErrors"></param>
    /// <returns></returns>
    public static async Task<bool> SendMailAccountInformation(IEmailTemplateRepository emailTemplateRepository, string email, string passwordGenerate, List<DetailError> detailErrors)
    {
        // Get the mail template
        var mailTemplate = await emailTemplateRepository.GetAccountInformationEmailTemplateAsync();
        var mailTitle = mailTemplate!.Title.Replace("${title}", mailTemplate.Title);
        
        // Replace the variables in the mail template
        var replacements = new Dictionary<string, string>
        {
            { "${email}", email },
            { "${password_generate}", passwordGenerate }
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