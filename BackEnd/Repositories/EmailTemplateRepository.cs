using BackEnd.Models;
using BackEnd.Models.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories;

public class EmailTemplateRepository : IEmailTemplateRepository
{
    private readonly AppDbContext _context;

    public EmailTemplateRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<VwEmailTemplateVerifyUser?> GetVerifyUserEmailTemplateAsync()
    {
        return await _context.VwEmailTemplateVerifyUsers.FirstOrDefaultAsync();
    }

    public async Task<VwEmailTemplateAccountInformation?> GetAccountInformationEmailTemplateAsync()
    {
        return await _context.VwEmailTemplateAccountInformations.FirstOrDefaultAsync();
    }

    public IEnumerable<SystemConfig> GetSystemConfigs()
    {
        return _context.SystemConfigs;
    }
}