using System.ComponentModel.DataAnnotations;

namespace dotnetMVP.Models.DTO.ChallengeDto
{
    public record ShowChallengeInfoDto
    {
        [Required(ErrorMessage = "ChallengeId is required.")]

        public Guid ChallengeId { get; init; }
        [Required(ErrorMessage = "Platform Id is required.")]
        public Guid PlatformId { get; init; }


        [MaxLength(20,ErrorMessage = "Platform name cannot exceed 50 characters.")]
        public string PlatformName { get; init; } = string.Empty;


        [MaxLength(50, ErrorMessage = "Challenge name cannot exceed 50 characters.")]
        public string Name { get; init; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Description cannot exceed 100 characters.")]
        public string ShortDescription { get; init; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Text cannot exceed 200 characters.")]
        public string Text { get; init; } = string.Empty;
    }
}
