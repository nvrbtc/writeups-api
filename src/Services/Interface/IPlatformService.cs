using dotnetMVP.Models.DTO.PlatformDto;
using dotnetMVP.Types;
using Nerdudes.Models.DTO;
namespace dotnetMVP.Services.Interface
{
    public interface IPlatformService
    {
        Task<ShowPlatformDto> CreateAsync(CreatePlatformDto dto);
        Task<IEnumerable<ShowPlatformDto>> GetAllAsync();
        Task<ServiceResult<ShowPlatformDto>> GetByIdAsync(Guid id);
        Task<ServiceResult<ShowPlatformDto>> UpdateAsync(ShowPlatformDto dto);
        Task<ServiceResult<GeneralResponse>> DeleteByIdAsync(Guid platformId, Guid userId);
    }
}
