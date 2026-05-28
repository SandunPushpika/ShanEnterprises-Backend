using Application.Interfaces.Repositories;
using Core.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories;

public class VerificationCodeRepository(AppDbContext context) : IVerificationCodeRepository
{
    public async Task AddVerificationCode(VerificationCodes code)
    {
        await context.VerificationCodes.AddAsync(code);
        await context.SaveChangesAsync();
    }

    public async Task UpdateVerificationCode(VerificationCodes code)
    {
        context.VerificationCodes.Update(code);
        await context.SaveChangesAsync();
    }

    public async Task<VerificationCodes?> GetVerificationCode(int userId)
    {
        return await context.VerificationCodes.FirstOrDefaultAsync(vc => vc.UserId == userId);
    }

    public async Task<VerificationCodes?> GetVerificationCodeByCode(string code)
    {
        return await context.VerificationCodes.FirstOrDefaultAsync(vc => vc.VerificationCode == code);
    }
}