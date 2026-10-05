using System.ComponentModel.DataAnnotations;

namespace Nerdudes.Models.DTO.User
{
    public record ChangePasswordDto
    {
        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; init; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; init; } = string.Empty;

        [Required(ErrorMessage = "Confirm new password is required.")]
        [Compare("NewPassword", ErrorMessage = "New password and confirm new password do not match.")]
        public string ConfirmNewPassword { get; init; } = string.Empty;
    }
}
