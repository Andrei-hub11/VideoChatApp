using Microsoft.AspNetCore.Mvc;

using VideoChatApp.Api.Errors;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Contracts.Request;

namespace VideoChatApp.Controllers;

[Route("api/v1")]
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
    public async Task<IActionResult> Register([FromBody] UserRegisterRequestDTO request)
    {
        var result = await _accountService.RegisterUserAsync(request);

        return result.Match(
            onSuccess: (user) => Ok(user), 
            onFailure: (errors) => errors.ToProblemDetailsResult());
    }

    //[HttpPost("login")]
    //public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO request)
    //{
    //    try
    //    {
    //        var user = await _accountService.LoginUserAync(request);

    //        return Ok(user);
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }
    //}
}
