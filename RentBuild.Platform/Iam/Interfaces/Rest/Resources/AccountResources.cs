using System.ComponentModel.DataAnnotations;
namespace RentBuild.Platform.Iam.Interfaces.Rest.Resources;

public sealed record SignUpResource([Required] string FirstName, [Required] string LastName, [Required, EmailAddress] string Email, [Required] string Password, [Required] string CompanyName, [Required] string Role);
public sealed record SignInResource([Required] string Email, [Required] string Password);
public sealed record RecoveryResource([Required, EmailAddress] string Email);
public sealed record ResetResource([Required] string Token, [Required] string Password);
public sealed record UserResource(int Id, string Email, string Role, string Status, string? Token);
