using BackEnd.Models;

namespace BackEnd.Repositories;

public interface IEmailTemplateRepository
{
    Task<VwEmailTemplateVerifyUser?> GetVerifyUserEmailTemplateAsync();
    IEnumerable<SystemConfig> GetSystemConfigs();
}