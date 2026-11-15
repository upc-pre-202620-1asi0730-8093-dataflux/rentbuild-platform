using RentBuild.Platform.Iam.Domain.Model.Commands;
using RentBuild.Platform.Shared.Application.Model;
namespace RentBuild.Platform.Iam.Application.CommandServices;

public sealed record AuthSession(int Id, string Email, string Role, string Status, string? Token = null);
public interface IUserCommandService
{
    Task<Result<AuthSession>> Handle(SignUpCommand command); Task<Result<AuthSession>> Handle(SignInCommand command); Task<Result<object>> Handle(RequestPasswordRecoveryCommand command); Task<Result<object>> Handle(ResetPasswordCommand command);
}