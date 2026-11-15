using RentBuild.Platform.Iam.Application.Internal.OutboundServices;
namespace RentBuild.Platform.Iam.Infrastructure.Hashing.BCrypt.Services;

public sealed class HashingService : IHashingService
{
    public string Hash(string password) => global::BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11); public bool Verify(string password, string hash) => global::BCrypt.Net.BCrypt.Verify(password, hash);
}
