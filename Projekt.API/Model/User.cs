using System.ComponentModel.DataAnnotations;

namespace Projekt.API.Model
{
    //Poniższy model odnosi się ogólnie do kont - więc używamy tego modelu do adminów i klientów
    public class User
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Podaj hasło!")]
        public required string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; }

        // Właściwość tylko dla klientów - admini nie będą raczej potrzebowali osobnego modelu na tę chwilę, ale TBD
        public virtual Client? Client { get; set; }
    }

    //enumerator do roli
    public enum UserRole
    {
        Client = 0,
        Administrator = 1
    }
}