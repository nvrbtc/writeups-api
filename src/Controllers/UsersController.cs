using dotnetMVP.Models.DTO.User;
using dotnetMVP.Services.Interface;
using dotnetMVP.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nerdudes.Models.DTO.User;

[Route("api/user")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _usrService;

    public UsersController(IUserService usrServ)
    {
        _usrService = usrServ;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto dto)
    {

        var result = await _usrService.CreateUserAsync(dto);

        return this.ResultToHttpCode(result);

    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto dto)
    {
        var result = await _usrService.ChangePasswordAsync(dto, this.GetUserIdFromClaims());
        return this.ResultToHttpCode(result);
    }

}
