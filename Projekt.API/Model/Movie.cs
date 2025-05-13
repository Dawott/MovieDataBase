using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.API.Model
{
    public class Movie
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public double Rating { get; set; } // Rating zostanie, ale będzie wartością przeciętną Ratingu
        public bool IsAvailable { get; set; } = true; //Zwykle będzie na true, ale na przykład platforma może stracić licencję tymczasowo
        public string? CoverImagePath { get; set; }

        //Nawigatory
        public ICollection<Client>? Clients { get; set; }    //do kontrolera jak cos
        public virtual ICollection<Rental>? Rentals { get; set; }
        public virtual ICollection<Rating>? Ratings { get; set; }
    }
}
