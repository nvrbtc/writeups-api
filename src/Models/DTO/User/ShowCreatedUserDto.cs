using System.ComponentModel.DataAnnotations;

namespace dotnetMVP.Models.DTO.User
{
    public record ShowCreatedUserDto
    {
        [Required(ErrorMessage = "Id is required.")]
        public Guid Id { get; init; }
        public string Username { get; init; } = string.Empty;
    }
}
