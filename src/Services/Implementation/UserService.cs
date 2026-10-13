using dotnetMVP.Models.DTO.User;
using dotnetMVP.Models.Entities;
using dotnetMVP.Services.Interface;
using dotnetMVP.Types;
using Microsoft.AspNetCore.Identity;
using Nerdudes.Models.DTO;
using Nerdudes.Models.DTO.User;
using Serilog.Context;

namespace dotnetMVP.Services.Realization
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly UserMapper _mapper;
        private readonly ILogger<UserService> _logger;
        public UserService( UserManager<AppUser> signInManager,
                            UserMapper mapper,
                            ILogger<UserService> logger
                            )
        {
            _userManager = signInManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<GeneralResponse>> ChangePasswordAsync(ChangePasswordDto dto, Guid userId)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null) return ServiceResult<GeneralResponse>.Fail("User not found", OperationResult.ObjectNotFound);

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (result.Succeeded) return ServiceResult<GeneralResponse>.Ok(new GeneralResponse { Message = "Password changed successfully" });

            return ServiceResult<GeneralResponse>.Fail(result.Errors.FirstOrDefault()?.Description ?? "Failed to change password", 
                                                                                                    OperationResult.InvalidInput);
        }

        public async Task<ServiceResult<ShowCreatedUserDto>> CreateUserAsync(CreateUserDto dto)
        {
            var user = _mapper.MapToEntity(dto);
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded) return ServiceResult<ShowCreatedUserDto>.Ok(_mapper.MapToDto(user));

            using (LogContext.PushProperty("User creation", "Failed"))
            {
                foreach (var error in result.Errors)
                    _logger.LogInformation("Error while creating user [{error}]", error);
            }
            if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                return ServiceResult<ShowCreatedUserDto>.Fail("Username already exists",
                                                            OperationResult.DuplicateObject);
            return ServiceResult<ShowCreatedUserDto>.Fail(result.Errors.FirstOrDefault()?.Description ?? "Failed to create user", 
                                                                                                        OperationResult.CreationFailed);
        }
    }
}
