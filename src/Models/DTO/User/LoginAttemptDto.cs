using System.ComponentModel.DataAnnotations;

namespace dotnetMVP.Models.DTO.User
{
    public record LoginAttemptDto
    {
        [Required(ErrorMessage ="Username is required.")]
        public string Username { get; init; } = string.Empty;


        [Required(ErrorMessage = "Password is reqiured.")]
        public string Password { get; init; } = string.Empty;
    }
}
