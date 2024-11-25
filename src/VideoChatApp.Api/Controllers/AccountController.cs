using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoChatApp.Api.Extensions;
using VideoChatApp.Api.Middleware;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Contracts.Request;

namespace VideoChatApp.Controllers;

[Route("api/v1/account")]
[ApiController]
[ServiceFilter(typeof(ResultFilter))]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IWebHostEnvironment _environment;

    public AccountController(IAccountService accountService, IWebHostEnvironment environment)
    {
        _accountService = accountService;
        _environment = environment;
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var accessToken = HttpContext
            .Request.Headers.Authorization.ToString()
            .Replace("Bearer ", "");

        var result = await _accountService.GetUserAsync(accessToken, cancellationToken);

        return result.Match(
            onSuccess: (user) => Ok(user),
            onFailure: (errors) => errors.ToProblemDetailsResult()
        );
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
    public async Task<IActionResult> Register(
        [FromBody] UserRegisterRequestDTO request,
        CancellationToken cancellationToken
    )
    {
        var result = await _accountService.RegisterUserAsync(request, cancellationToken);

        
        return result.Match(
            onSuccess: (authResponse) => Ok(authResponse),
            onFailure: (errors) => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] UserLoginRequestDTO request,
        CancellationToken cancellationToken
    )
    {
        var result = await _accountService.LoginUserAsync(request, cancellationToken);

        return result.Match(
            onSuccess: (authResponse) => Ok(authResponse),
            onFailure: (errors) => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgetPasswordRequestDTO request,
        CancellationToken cancellationToken
    )
    {
        var result = await _accountService.ForgotPasswordAsync(request, cancellationToken);

        return result.Match(
            onSuccess: (response) => Ok(response),
            onFailure: (errors) => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost("token-renew")]
    public async Task<IActionResult> RefreshAccessToken(
        [FromBody] UpdateAccessTokenRequestDTO request,
        CancellationToken cancellationToken
    )
    {
        var result = await _accountService.UpdateAccessTokenAsync(request, cancellationToken);

        return result.Match(
            onSuccess: (accessToken) => Ok(accessToken),
            onFailure: (errors) => errors.ToProblemDetailsResult()
        );
    }

    [Authorize]
    [HttpPut("profile/{userId}")]
    public async Task<IActionResult> UpdateUserAsync(
        [FromBody] UpdateUserRequestDTO request,
        string userId,
        CancellationToken cancellationToken
    )
    {
        var result = await _accountService.UpdateUserAsync(userId, request, cancellationToken);

        return result.Match(
            onSuccess: (user) => Ok(user),
            onFailure: (errors) => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("update-password")]
    public async Task<IActionResult> UpdateUserPassword(
        [FromBody] UpdatePasswordRequestDTO request,
        CancellationToken cancellationToken
    )
    {
        var result = await _accountService.UpdateUserPasswordAsync(request, cancellationToken);

        return result.Match(
            onSuccess: (response) => Ok(response),
            onFailure: (errors) => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("test-cleanup")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> CleanupTestUsers(CancellationToken cancellationToken)
     {
        if (!_environment.IsDevelopment() && !_environment.IsEnvironment("Testing"))
        {
            return NotFound();
        }

        await _accountService.CleanupTestUsersAsync(cancellationToken);
        return Ok();
    }
}
