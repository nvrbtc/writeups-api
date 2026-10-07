using dotnetMVP.Models.DTO.User;
using dotnetMVP.Models.Entities;
using dotnetMVP.Types;
using Nerdudes.Models.DTO;
using Nerdudes.Models.DTO.User;

namespace dotnetMVP.Services.Interface
{
    public interface IUserService
    {
        //Add more methods 
        Task<ServiceResult<ShowCreatedUserDto>> CreateUserAsync(CreateUserDto dto);
        Task<ServiceResult<GeneralResponse>> ChangePasswordAsync(ChangePasswordDto dto,Guid userId);
    }
}
