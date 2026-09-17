using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IVerificationCodeRepository
{
    Task AddVerificationCode(VerificationCodes code);
    Task<VerificationCodes?> GetVerificationCode(int userId);
    Task<VerificationCodes?> GetVerificationCodeByCode(string code);
    Task UpdateVerificationCode(VerificationCodes code);

}