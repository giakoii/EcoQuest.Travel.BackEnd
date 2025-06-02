using BackEnd.Models;

namespace BackEnd.Repositories;

public interface IEmailTemplateRepository
{
    Task<VwEmailTemplateVerifyUser?> GetVerifyUserEmailTemplateAsync();
    
    Task<VwEmailTemplateAccountInformation?> GetAccountInformationEmailTemplateAsync();
    
    IEnumerable<SystemConfig> GetSystemConfigs();
}