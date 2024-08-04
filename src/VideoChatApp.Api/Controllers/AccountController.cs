using Microsoft.AspNetCore.Mvc;

using VideoChatApp.Api.Extensions;
using VideoChatApp.Application.Common.Result;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Contracts.Request;

namespace VideoChatApp.Controllers;

[Route("api/v1/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    //[Authorize(Policy = "Admin")]
    //[HttpGet("users")]
    //public async Task<IActionResult> GetAllUsers()
    //{
    //    try
    //    {
    //        var users = await _accountService.GetAllUsersAsync();

    //        return Ok(new
    //        {
    //            Users = users
    //        });
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }
    //}

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequestDTO request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.RegisterUserAsync(request, cancellationToken);

        return result.Match(
            onSuccess: (user) => Ok(user),
            onFailure: (errors) => errors.ToProblemDetailsResult());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO request,
        CancellationToken cancellationToken)
    {

        var result = await _accountService.LoginUserAsync(request, cancellationToken);

        return result.Match(
            onSuccess: (user) => Ok(user),
            onFailure: (errors) => errors.ToProblemDetailsResult());
    }
}
