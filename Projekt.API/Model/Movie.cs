using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.API.Model
{
    public class Movie
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public double Rating { get; set; }

        [ForeignKey("ClientID")]
        public required int ClientID { get; set; }



        public ICollection<Client>? Clients { get; set; }    //do kontrolera jak cos

        public string? CoverImagePath { get; set; }
    }
}
