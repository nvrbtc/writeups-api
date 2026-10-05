using dotnetMVP.Models.DTO.PlatformDto;
using dotnetMVP.Services.Interface;
using dotnetMVP.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace dotnetMVP.Controllers
{
    [Authorize]
    [Route("api/platform")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformService _platformService;

        public PlatformController(IPlatformService serv)
        {
            _platformService = serv;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ShowPlatformDto>>> GetAllAsync()
        {
            var result = await _platformService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("id/{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _platformService.GetByIdAsync(id);

            return this.ResultToHttpCode(result);
        }

        //TODO: [Authorize(Roles = "Admin, Moderator")] - add roles when they will be introduced
        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromBody]CreatePllatformDto dto)
        {
            return Ok(await _platformService.CreateAsync(dto));
        }

        //TODO: [Authorize(Roles = "Admin, Moderator")] - add roles when they will be introduced
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync([FromBody] ShowPlatformDto dto)
        {
            var result = await _platformService.UpdateAsync(dto);

            return this.ResultToHttpCode(result);
        }

        //TODO: [Authorize(Roles = "Admin, Moderator")] - add roles when they will be introduced
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _platformService.DeleteByIdAsync(id);

            return Ok();
        }

    }
}
