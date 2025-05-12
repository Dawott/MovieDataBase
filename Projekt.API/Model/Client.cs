using System.ComponentModel.DataAnnotations;

namespace Projekt.API.Model
{
    public class Client
    {
        public int ID { get; set; }

        
        public string Name { get; set; }

        [Required(ErrorMessage = "Client lastname is required")]
        public required string Lastname { get; set; }

        public virtual ICollection<Movie>?Movies { get; set; }   //odnosnik do kontrolera
    }
}
