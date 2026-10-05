using dotnetMVP.Models;
using dotnetMVP.Models.DTO.User;
using dotnetMVP.Models.Entities;
using dotnetMVP.Services.Interface;
using dotnetMVP.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace dotnetMVP.Services.Realization
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDBcontext _context;
        private readonly IJwtHandler _jwt;
        private readonly UserManager<AppUser> _usrMng;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDBcontext db,
                            IJwtHandler jwt,
                            UserManager<AppUser> mng,
                            ILogger<AuthService> logger)
        {
            _context = db;
            _jwt = jwt;
            _usrMng = mng;
            _logger = logger;
        }
        public async Task<ServiceResult<JwtTokens>> LoginAsync(LoginAttempDto loginAttempDto)
        {
            var user = await _context.AppUser.Where(x => x.UserName == loginAttempDto.Username).SingleOrDefaultAsync();

            if (user == null) return ServiceResult<JwtTokens>.Fail("Incorrect credentials.",
                                                                    OperationResult.FailedAuth);

            if (!await _usrMng.CheckPasswordAsync(user, loginAttempDto.Password))
            {
                _logger.LogInformation("Failed login attempt for id = {userId}, username = {username}",
                                        user.Id, user.UserName);
                return ServiceResult<JwtTokens>.Fail("Incorrect credentials.",
                                                    OperationResult.FailedAuth);
            }

            var roles = await _usrMng.GetRolesAsync(user);
            var accessToken = _jwt.GenerateJwtToken(new UserClaims
            {
                Id = user.Id,
                Claims = roles
            });

            var refreshToken = _jwt.GenerateRefreshToken();
            string hashedRefreshToken = HashToHexLowerString(refreshToken);
            // Store the hashed refresh token in the database
            
            await _context.RefreshTokens.AddAsync(new RefreshToken()
            {
                RefreshTokenId = Guid.NewGuid(),
                RefreshSecret = hashedRefreshToken, // store the hashed refresh token
                Expires = DateTime.UtcNow.AddDays(10),
                UserId = user.Id
            });

            await _context.SaveChangesAsync();
            _logger.LogInformation("Succesful JWT generation for user = {userId}", user.Id);

            return ServiceResult<JwtTokens>.Ok(new JwtTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken // return raw[non-hashed] refresh token to end user
            });
        }


        public async Task<ServiceResult<JwtTokens>> RefreshAsync(JwtTokens tokens)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = _jwt.GetPrincipalFromExpiredToken(tokens.AccessToken);
            }
            catch(SecurityTokenException ex)
            {
                _logger.LogWarning("Failed JWT refreshing: {message}", ex.Message);
                return ServiceResult<JwtTokens>.Fail("Failed JWT refreshing", OperationResult.FailedAuth);
            }
            var userIdString = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (userIdString == null) return ServiceResult<JwtTokens>.Fail("Failed JWT refreshing", OperationResult.FailedAuth);

            var userId = Guid.Parse(userIdString);
            var user = await _usrMng.FindByIdAsync(userId.ToString());

            if (user == null) return ServiceResult<JwtTokens>.Fail("User does not exist",OperationResult.FailedAuth);

            var hashedUserRefreshToken = HashToHexLowerString(tokens.RefreshToken);
            var currentRefresh = await _context.RefreshTokens.Where(x => x.UserId == userId
                                                                    && x.RefreshSecret == hashedUserRefreshToken) // maybe there is a better way to compare if two hashes are equal?
                                                                    .FirstOrDefaultAsync();
            
            if (currentRefresh == null || currentRefresh.Expires < DateTime.UtcNow)
            {
                _logger.LogWarning("Attempt to use expired/wrong refresh token for userId = {userId}, username = {username}",
                                                                                            user.Id, user.UserName);
                return ServiceResult<JwtTokens>.Fail("Expired token.",
                                                     OperationResult.FailedAuth);
            }

            var newRefreshSecret = _jwt.GenerateRefreshToken();
            var roles = await _usrMng.GetRolesAsync(user);

            var accessToken = _jwt.GenerateJwtToken(new()
            {
                Id = userId,
                Claims = roles
            });

            
            currentRefresh.RefreshSecret = HashToHexLowerString(newRefreshSecret);
            currentRefresh.Expires = DateTime.UtcNow.AddDays(10);

            await _context.SaveChangesAsync();

            return ServiceResult<JwtTokens>.Ok(new JwtTokens
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshSecret
            });

        }
        private string HashToHexLowerString(string input)
        {
            return Convert.ToHexStringLower(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(input)));
        }
    }
}
