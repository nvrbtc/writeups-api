using dotnetMVP.Models.DTO.ChallengeDto;
using dotnetMVP.Types;
using Nerdudes.Models.DTO;

namespace dotnetMVP.Services.Interface
{
    public interface IChallengeService
    {
        // Add delete method 
        Task<IEnumerable<ShowChallengeInfoDto>> GetAllAsync();
        Task<ServiceResult<ShowChallengeInfoDto>> GetByIdAsync(Guid id);
        Task<IEnumerable<ShowChallengeInfoDto>> FilterByPlatformIdAsync(Guid platformId);
        Task<ShowChallengeInfoDto> CreateAsync(CreateChallengeDto dto);
        Task<ServiceResult<ShowChallengeInfoDto>> UpdateAsync(ShowChallengeInfoDto dto);
        Task<ServiceResult<GeneralResponse>> DeleteAsync(Guid challengeId, Guid userId);
    }
}
