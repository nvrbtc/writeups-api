using dotnetMVP.Models.DTO.User;
using dotnetMVP.Types;

namespace dotnetMVP.Services.Interface
{
    public interface IAuthService
    {
        // Consider logout method with proper black listing of refresh/access tokens
        Task<ServiceResult<JwtTokens>> LoginAsync(LoginAttempDto loginAttempDto);
        Task<ServiceResult<JwtTokens>> RefreshAsync(JwtTokens tokens);
    }
}
