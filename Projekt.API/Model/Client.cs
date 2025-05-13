using System.ComponentModel.DataAnnotations;

namespace Projekt.API.Model
{
    public class Client
    {
        public int ID { get; set; }

        
        public string Name { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public required string Lastname { get; set; }

        public int UserID { get; set; } //FK do Modelu User

        // Właściwości "nawigacyjne" do relacji - User (Model główny do kont), Movie (do filmów, które są na koncie klienta), Rental (wypożyczenia), Ratings (oceny poszczególnych userów)
        public virtual User User { get; set; }
        public virtual ICollection<Movie>?Movies { get; set; }   //odnosnik do kontrolera
        public virtual ICollection<Rental>? Rentals { get; set; }
        public virtual ICollection<Rating>? Ratings { get; set; }
    }
}
