using Devpull.Controllers;
using Devpull.Controllers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Devpull.Users;

[Route("api/auth")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ExecutionService _executionService;
    private readonly UserService _userService;

    public UserController(ExecutionService executionService, UserService userService)
    {
        _executionService = executionService;
        _userService = userService;
    }

    [HttpPost("get-user-info")]
    public Task<OperationResult<UserInfo>> GetUserInfo()
    {
        return _executionService.TryExecute(
            () => _userService.GetUserInfo(HttpContext.RequestAborted)
        );
    }

    [HttpPost("register")]
    public Task<OperationResult<bool>> Register([FromBody] RegisterModel model)
    {
        return _executionService.TryExecute(
            () => _userService.Register(model, HttpContext.RequestAborted)
        );
    }

    [HttpPost("confirm-email")]
    public Task<OperationResult<TokensModel>> ConfirmEmail([FromBody] ConfirmEmailModel model)
    {
        return _executionService.TryExecute(
            () => _userService.ConfirmEmail(model, HttpContext.RequestAborted)
        );
    }

    [HttpPost("login")]
    public async Task<OperationResult<TokensModel>> Login([FromBody] LoginModel model)
    {
        // можно сделать валидацию чисто полей модели - включить фильтр SuppressModelStateInvalidFilter и использовать ModelState
        // сразу будет выдаваться ошибка не заходя в бизнес логику
        // может быть полезно при DDOS атаках
        return await _executionService.TryExecute(
            () => _userService.Login(model, HttpContext.RequestAborted)
        );
    }

    [HttpPost("logout")]
    public Task<OperationResult<int>> Logout(LogoutModel model)
    {
        return _executionService.TryExecute(
            () => _userService.Logout(model, HttpContext.RequestAborted)
        );
    }

    [HttpPost("refresh-tokens")]
    public Task<OperationResult<TokensModel>> RefreshTokens([FromBody] RefreshTokenModel model)
    {
        return _executionService.TryExecute(
            () => _userService.RefreshTokens(model, HttpContext.RequestAborted)
        );
    }

    [HttpPost("forgot-password")]
    public Task<OperationResult<bool>> ForgotPassword(ForgotPasswordModel model)
    {
        return _executionService.TryExecute(
            () => _userService.ForgotPassword(model, HttpContext.RequestAborted)
        );
    }

    [HttpPost("reset-password")]
    public Task<OperationResult<TokensModel>> ResetPassword(ResetPasswordModel model)
    {
        return _executionService.TryExecute(
            () => _userService.ResetPassword(model, HttpContext.RequestAborted)
        );
    }

    [HttpPost("test")]
    public Task<OperationResult<bool>> Test()
    {
        return _executionService.TryExecute(() => _userService.Test());
    }
}
