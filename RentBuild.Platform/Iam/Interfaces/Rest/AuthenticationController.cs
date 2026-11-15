using RentBuild.Platform.Iam.Application.QueryServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RentBuild.Platform.Iam.Application.CommandServices;
using RentBuild.Platform.Iam.Domain.Model.Commands;
using RentBuild.Platform.Iam.Domain.Repositories;
using RentBuild.Platform.Iam.Interfaces.Rest.Resources;
using RentBuild.Platform.Iam.Interfaces.Rest.Transform;
using RentBuild.Platform.Shared.Interfaces.Rest;
using RentBuild.Platform.Shared.Application.OutboundServices;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Iam.Interfaces.Rest;

[ApiController, Route("api/v1/authentication")]
public sealed class AuthenticationController(IUserCommandService commands, IUserQueryService queries) : ControllerBase
{
    [AllowAnonymous, HttpPost("sign-up")] public async Task<IActionResult> SignUp(SignUpResource input) => (await commands.Handle(AccountAssembler.Command(input))).ToAction(AccountAssembler.Resource, 201);
    [AllowAnonymous, HttpPost("sign-in")] public async Task<IActionResult> SignIn(SignInResource input) => (await commands.Handle(AccountAssembler.Command(input))).ToAction(AccountAssembler.Resource);
    [AllowAnonymous, HttpPost("password-recovery")] public async Task<IActionResult> Recover(RecoveryResource input) => (await commands.Handle(new RequestPasswordRecoveryCommand(input.Email))).ToAction(r => r, 202);
    [AllowAnonymous, HttpPost("reset-password")] public async Task<IActionResult> Reset(ResetResource input) => (await commands.Handle(new ResetPasswordCommand(input.Token, input.Password))).ToAction(r => r);
    [Authorize, HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await queries.Current();
        return Ok(new UserResource(user.Id, user.Email, user.Role, user.Status, null));
    }
}