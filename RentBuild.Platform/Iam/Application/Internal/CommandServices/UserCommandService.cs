using System.Security.Cryptography;
using System.Text;
using RentBuild.Platform.Iam.Application.CommandServices;
using RentBuild.Platform.Iam.Application.Internal.OutboundServices;
using RentBuild.Platform.Iam.Domain.Model.Aggregates;
using RentBuild.Platform.Iam.Domain.Model.Commands;
using RentBuild.Platform.Iam.Domain.Model.ValueObjects;
using RentBuild.Platform.Iam.Domain.Repositories;
using RentBuild.Platform.Shared.Application.Internal;
using RentBuild.Platform.Shared.Application.Model;
using RentBuild.Platform.Shared.Domain.Repositories;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Iam.Application.Internal.CommandServices;

public sealed class UserCommandService(IUserRepository users, IPasswordResetTokenRepository resets, IHashingService hashing, ITokenService tokens, IProfileRegistration profiles, IRecoveryMailSender mail, IUnitOfWork work, CommandExecutor executor) : IUserCommandService
{
    static void Password(string value) => Rule.Require(value is not null && value.Length >= 8 && Encoding.UTF8.GetByteCount(value) <= 72, "Password must contain at least 8 characters and at most 72 UTF-8 bytes");
    public Task<Result<AuthSession>> Handle(SignUpCommand c) => executor.ExecuteAsync(async () =>
    {
        Password(c.Password);
        var email = new EmailAddress(c.Email).Value;
        Rule.Require(!(await users.ListAsync()).Any(u => u.Email == email), "Email already registered", ErrorKind.Conflict, "DUPLICATE_EMAIL");
        var user = new User(email, hashing.Hash(c.Password), c.Role);
        await users.AddAsync(user);
        await work.FlushAsync();
        await profiles.Create(user.Id, c.FirstName, c.LastName, email, c.CompanyName);
        return new AuthSession(user.Id, user.Email, user.Role, user.Status);
    });
    public Task<Result<AuthSession>> Handle(SignInCommand c) => executor.ExecuteAsync(async () =>
    {
        Rule.Require(c.Password is not null && Encoding.UTF8.GetByteCount(c.Password) <= 72, "Invalid email or password", ErrorKind.Unauthorized);
        var email = (c.Email ?? "").Trim().ToLowerInvariant();
        var user = (await users.ListAsync()).FirstOrDefault(u => u.Email == email);
        Rule.Require(user is not null && hashing.Verify(c.Password!, user.PasswordHash) && user.Status == "active", "Invalid email or password", ErrorKind.Unauthorized, "INVALID_CREDENTIALS");
        return new AuthSession(user!.Id, user.Email, user.Role, user.Status, tokens.Issue(user));
    });
    public Task<Result<object>> Handle(RequestPasswordRecoveryCommand c) => executor.ExecuteAsync<object>(async () =>
    {
        var email = new EmailAddress(c.Email).Value;
        Rule.Require(mail.IsConfigured, "Password recovery requires SMTP configuration", ErrorKind.Unavailable, "MAIL_NOT_CONFIGURED");
        var user = (await users.ListAsync()).FirstOrDefault(u => u.Email == email && u.Status == "active");
        if (user is not null)
        {
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            await resets.AddAsync(new(user.Id, HashToken(token), DateTimeOffset.UtcNow.AddMinutes(15)));
            await mail.Send(email, token);
        }
        return new
        {
            message = "If the account exists, recovery instructions will be sent"
        };
    });
    public Task<Result<object>> Handle(ResetPasswordCommand c) => executor.ExecuteAsync<object>(async () =>
    {
        Password(c.Password);
        Rule.Require(!string.IsNullOrWhiteSpace(c.Token) && c.Token.Length <= 256, "Invalid recovery token");
        var hash = HashToken(c.Token);
        var reset = (await resets.ListAsync()).FirstOrDefault(t => t.Hash == hash);
        Rule.Require(reset is not null, "Invalid or expired recovery token");
        reset!.Consume(DateTimeOffset.UtcNow);
        var user = Rule.Found(await users.FindByIdAsync(reset.UserId), "Account");
        user.ChangePassword(hashing.Hash(c.Password));
        foreach (var other in (await resets.ListAsync()).Where(t => t.UserId == user.Id && !t.Used && t.ExpiresAt > DateTimeOffset.UtcNow))
            other.Consume(DateTimeOffset.UtcNow);
        return new
        {
            message = "Password updated; sign in again"
        };
    });
    private static string HashToken(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
