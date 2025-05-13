using System.ComponentModel.DataAnnotations;

namespace Projekt.API.DTOs
{
    public class MoviesDTO
    {
        public class RateMovieDto
        {
            [Required]
            [Range(1, 10, ErrorMessage = "Ocena musi być w zakresie 1 do 10")]
            public double Rating { get; set; }

            public string? Comment { get; set; }
        }

        public class RentalDto
        {
            public int Id { get; set; }
            public int MovieId { get; set; }
            public string MovieName { get; set; }
            public string MovieType { get; set; }
            public DateTime RentalDate { get; set; }
            public DateTime? ReturnDate { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
