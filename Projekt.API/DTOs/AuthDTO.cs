using System.ComponentModel.DataAnnotations;

namespace Projekt.API.DTOs
{
    public class AuthDTO
    {
        public class LoginDto
        {
            [Required]
            [EmailAddress]
            public required string Email { get; set; }

            [Required]
            public required string Password { get; set; }
        }

        public class RegisterDto
        {
            [Required]
            [EmailAddress]
            public required string Email { get; set; }

            [Required]
            [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków")]
            public required string Password { get; set; }

            [Required]
            public required string Name { get; set; }

            [Required]
            public required string Lastname { get; set; }
        }

        public class LoginResponseDto
        {
            public required string Token { get; set; }
            public required string Email { get; set; }
            public required string Role { get; set; }
            public int? ClientId { get; set; }  // Ponieważ admin nie ma ClientId = może być null
            public DateTime ExpiresAt { get; set; }
        }

        public class UserDto
        {
            public int Id { get; set; }
            public required string Email { get; set; }
            public required string Role { get; set; }
            public ClientDto? Client { get; set; }
        }

        public class ClientDto
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public required string Lastname { get; set; }
        }

        public class ChangePasswordDto
        {
            [Required]
            public required string CurrentPassword { get; set; }

            [Required]
            [MinLength(6, ErrorMessage = "Nowe hasło musi mieć co najmniej 6 znaków")]
            public required string NewPassword { get; set; }
        }
    }
}
