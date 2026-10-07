using dotnetMVP.Models;
using dotnetMVP.Models.DTO.Writeup;
using dotnetMVP.Types;
using Nerdudes.Models.DTO;

namespace dotnetMVP.Services.Interface
{
    public interface IWriteUpService
    {
        Task<IEnumerable<ShowWriteUpDto>> GetAllAsync();
        Task<IEnumerable<ShowWriteUpDto>> FilterByChallengeNameAsync(string name);
        Task<IEnumerable<ShowWriteUpDto>> FilterByPlatformIdAsync(Guid platformId);
        Task<ServiceResult<IEnumerable<ShowWriteUpDto>>> GetAllByUserIdAsync(Guid userId);
        Task<ServiceResult<ShowWriteUpDto>> GetByIdAsync(Guid writeupId);
        Task<ServiceResult<ShowWriteUpDto>> CreateAsync(CreateWriteUpDto dto,Guid id);
        Task<ServiceResult<ShowWriteUpDto>> UpdateAsync(ShowWriteUpDto dto,Guid userId);
        Task<ServiceResult<GeneralResponse>> DeleteAsync(Guid writeupId,Guid userId);

    }
}
