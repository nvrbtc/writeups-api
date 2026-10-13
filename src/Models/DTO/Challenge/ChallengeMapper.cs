using dotnetMVP.Models.Entities;

namespace dotnetMVP.Models.DTO.ChallengeDto
{
    public class ChallengeMapper
    {
        public Challenge MapToEntity(CreateChallengeDto dto)
        {
            return new Challenge
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Text = dto.Text,
                ShortDescription = dto.ShortDescription,
                PlatformId = dto.PlatformId
            };
        }

        public Challenge MapToEntity(ShowChallengeInfoDto dto, Challenge existingEntity)
        { 
            existingEntity.Name = dto.Name;
            existingEntity.Text = dto.Text;
            existingEntity.ShortDescription = dto.ShortDescription;

            return existingEntity;
        }
        public ShowChallengeInfoDto MapFromEntity(Challenge entity)
        {
            return new()
            {
                ChallengeId = entity.Id,
                Name = entity.Name,
                Text = entity.Text,
                PlatformId = entity.PlatformId,
                PlatformName = entity.Platform?.Name ?? "Placeholder",
                ShortDescription = entity.ShortDescription
            };
        }
    }
}

