using System.ComponentModel.DataAnnotations;

namespace dotnetMVP.Models.DTO.ChallengeDto
{
    public record CreateChallengeDto
    {
        [Required(ErrorMessage = "PlatformId is required")]
        public Guid PlatformId { get; init; }

        [Required]
        [MaxLength(50, ErrorMessage = "Title cannot be longer than 200 characters")]
        public string Name { get; init; } = string.Empty;

        [MaxLength(50,ErrorMessage = "Description cannot be longer than 200 characters")]
        public string ShortDescription { get; init; } = string.Empty;

        [MaxLength(500,ErrorMessage = "Text cannot be longer than 2000 characters")]
        public string Text { get; init;  } = string.Empty;
    }
}
