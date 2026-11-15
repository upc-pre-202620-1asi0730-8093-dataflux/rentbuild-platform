namespace RentBuild.Platform.Iam.Domain.Model.Commands;

public sealed record SignUpCommand(string FirstName, string LastName, string Email, string Password, string CompanyName, string Role);
public sealed record SignInCommand(string Email, string Password);
public sealed record RequestPasswordRecoveryCommand(string Email);
public sealed record ResetPasswordCommand(string Token, string Password);