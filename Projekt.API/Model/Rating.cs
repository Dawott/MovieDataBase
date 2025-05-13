using System.ComponentModel.DataAnnotations;

namespace Projekt.API.Model
{
    //Rating będzie miał osobny model, ponieważ klient może wypożyczyć filmy kilka razy + oddzielamy film od wypożyczenia filmu
    public class Rating
    {
        public int ID { get; set; }

        public int ClientID { get; set; }
        public int MovieID { get; set; }

        [Range(1, 10, ErrorMessage = "Ocena musi mieścić się między 1 a 10")]
        public double Value { get; set; }

        public string? Comment { get; set; }
        public DateTime RatedAt { get; set; }

        // Nawigacje
        public virtual Client Client { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
