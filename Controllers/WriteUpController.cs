using dotnetMVP.Models.DTO.Writeup;
using dotnetMVP.Services.Interface;
using dotnetMVP.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace dotnetMVP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/writeups")]
    public class WriteUpController : ControllerBase
    {

        private readonly IWriteUpService _writeUpService;
        
        public WriteUpController(IWriteUpService writeUpService)
        {
            _writeUpService = writeUpService;
        }



        [HttpGet("all")]
        public async Task<IActionResult> GetAllWriteUps()
        {
            var result = await _writeUpService.GetAllAsync();
            return Ok(result);
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchQuery([FromQuery(Name = "q")] string query)
        {
            var result = await _writeUpService.FilterByChallengeNameAsync(query);
            return Ok(result);
        }


        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateWriteUpDto writeup)
        {
            var result = await _writeUpService.CreateAsync(writeup,this.GetUserIdFromClaims());
            return this.ResultToHttpCode(result);
            
        }


        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromBody] ShowWriteUpDto dto)
        {
            var result = await _writeUpService.EditAsync(dto, this.GetUserIdFromClaims());
            return this.ResultToHttpCode(result);
        }


        [HttpDelete("delete/{writeupId:guid}")]
        public async Task<IActionResult> Delete(Guid writeupId)
        {
            var result = await _writeUpService.DeleteAsync(writeupId,
                                                    this.GetUserIdFromClaims());

            
            return this.ResultToHttpCode(result);
        }


        [HttpGet("id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _writeUpService.GetByIdAsync(id);
            return this.ResultToHttpCode(result);
        }


        [HttpGet("user/{id:guid}")]
        public async Task<IActionResult> GetByUserIdAsync(Guid id) 
        { 
            var result = await _writeUpService.GetAllByUserIdAsync(id);
            return this.ResultToHttpCode(result);
        }


        [HttpGet("mywriteups")]
        public async Task<ActionResult<IEnumerable<ShowWriteUpDto>>> GetMyWriteUps()
        {
            var result = await _writeUpService.GetAllByUserIdAsync(this.GetUserIdFromClaims());
            
            return Ok(result);
        }
    }
}
