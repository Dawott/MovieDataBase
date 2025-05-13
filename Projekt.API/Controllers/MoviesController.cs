using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.API.Model;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using static Projekt.API.DTOs.MoviesDTO;

namespace Projekt.API.Controllers
{
    [Route("api/[Controller]/[action]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly MoviesDBContext _db;
        private readonly ILogger<MoviesController> _logger;

       public MoviesController(MoviesDBContext context, ILogger<MoviesController> logger)
        {
            _db = context;
            _logger = logger;
        }

        // GET: api/Movies/list
        [ActionName("List")]
        [HttpGet]
        [AllowAnonymous] //Wszyscy mogą widzieć listę filmów
        public async Task<ActionResult<List<Movie>>> GetListAsync()
        {
            try
            {
                await Task.Delay(1000);
                var movies = await _db.Movies
                     .Include(m => m.Ratings)
                     .ToListAsync();
                //Kalkulacja Ratingu - ale do sprawdzenia, czy nie można tego jakoś mądrzej zliczać
                foreach (var movie in movies)
                {
                    if (movie.Ratings != null && movie.Ratings.Any())
                    {
                        movie.Rating = Math.Round(movie.Ratings.Average(r => r.Value), 1);
                    }
                }

                return Ok(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pojawił się błąd w zwracaniu filmów");
                return StatusCode(500, "Pojawił się błąd w zwracaniu filmów");
            }
        }

        // GET: api/Movies/ByID/1
        [ActionName("ByID")]
        [HttpGet] //("{id:range(1,200)}") - range niepotrzebny
        [AllowAnonymous]
        public async Task<ActionResult<Movie>> GetByID(int id)
        {
            var movie = await _db.Movies
                .Include(m => m.Ratings)
                    .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (movie == null)
                return NotFound($"Film z ID {id} nie istnieje!");
            //Tak samo liczenie ratingu - jak wyżej
            if (movie.Ratings != null && movie.Ratings.Any())
            {
                movie.Rating = Math.Round(movie.Ratings.Average(r => r.Value), 1);
            }
            return Ok(movie);
        }

        // POST: api/Movies/Add
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ActionName("Add")]
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<int>> PostAdd([FromBody] Movie newMovie)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingMovie = await _db.Movies.FirstOrDefaultAsync(m => m.Name == newMovie.Name);
            if (existingMovie != null)
                return Conflict($"Film pod nazwą {newMovie.Name} już istnieje!");

            newMovie.IsAvailable = true; // By default nowe filmy będą dostępne
            _db.Movies.Add(newMovie);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Dodano film: {newMovie.Name} (ID: {newMovie.ID})");
            return Ok(newMovie.ID);

        }

        // DELETE: api/Movies/5
        [ActionName("Delete")]
        [HttpDelete("{id}")] //Precyzuję tutaj ID
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Delete(int id)
        {
            var movie = _db.Movies.Find(id);
            if (movie == null)
                return NotFound($"Film z ID {id} nie istnieje!");

            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Usunięto film: {movie.Name} (ID: {id})");
            return Ok();
        }

        //api/Movie/Update
        [ActionName("Update")]
        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> PutUpdate([FromBody] Movie updatedMovie)
        {
            var movie = await _db.Movies.FindAsync(updatedMovie.ID);
            if (movie == null)
                return NotFound($"Film {updatedMovie.Name} nie istnieje!");

            // Aktualizuj WSZYSTKIE pola
            movie.Name = updatedMovie.Name;
            movie.Type = updatedMovie.Type;
            movie.IsAvailable = updatedMovie.IsAvailable;
            //movie.Rating = updatedMovie.Rating;
            //movie.ClientID = updatedMovie.ClientID;

            await _db.SaveChangesAsync();
            _logger.LogInformation($"Uaktualniono film: {movie.Name} (ID: {movie.ID})");
            return Ok();
        }

        private bool MovieExists(int id)
        {
            return _db.Movies.Any(e => e.ID == id);
        }

        [ActionName("UploadCover")]
        [HttpPost("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UploadCover(int id, IFormFile file)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest("Nie przesłano pliku.");

            if (file.Length > 5_000_000)
                return BadRequest("Plik jest zbyt duży (max 5MB)");

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Covers");
            Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            movie.CoverImagePath = fileName;
            await _db.SaveChangesAsync();

            return Ok(new { fileName });
        }

        [ActionName("DownloadCover")]
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult DownloadCover(int id)
        {
            var movie = _db.Movies.Find(id);
            if (movie?.CoverImagePath == null) return NotFound();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Covers", movie.CoverImagePath);
            if (!System.IO.File.Exists(path)) return NotFound();

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
                contentType = "application/octet-stream";

            var fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, contentType, movie.CoverImagePath);
        }


        [ActionName("DeleteCover")]
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCover(int id)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null)
                return NotFound("Nie znaleziono filmu!");

            if (string.IsNullOrEmpty(movie.CoverImagePath))
                return BadRequest("Ten film nie ma okładki do usunięcia.");

            // Ścieżka do pliku
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Covers", movie.CoverImagePath);

            // Usuń plik, jeśli istnieje
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            // Usuń informację z bazy
            movie.CoverImagePath = null;
            await _db.SaveChangesAsync();

            return Ok("Okładka została usunięta.");
        }

        // NOWE ZAPYTANIA
        // Wypożyczenie
        [ActionName("Rent")]
        [HttpPost("{id}")]
        [Authorize(Policy = "ClientOnly")] // Tylko klienci mogą wypożyczać
        public async Task<ActionResult> RentMovie(int id)
        {
            var clientIdClaim = User.FindFirst("clientId")?.Value;
            if (string.IsNullOrEmpty(clientIdClaim))
                return BadRequest("Brak ID");

            var clientId = int.Parse(clientIdClaim);
            var movie = await _db.Movies.FindAsync(id);

            if (movie == null)
                return NotFound("Brak takiego filmu");

            if (!movie.IsAvailable)
                return BadRequest("Film niedostępny do wypożyczenia");

            // Sprawdzanie czy klient już wypożycza
            var activeRental = await _db.Rentals
                .FirstOrDefaultAsync(r => r.ClientID == clientId &&
                                         r.MovieID == id &&
                                         r.ReturnDate == null);

            if (activeRental != null)
                return BadRequest("Masz już ten film wypożyczony");

            var rental = new Rental
            {
                ClientID = clientId,
                MovieID = id,
                RentalDate = DateTime.UtcNow
            };

            _db.Rentals.Add(rental);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Film wypożyczony: {movie.Name} do ID: {clientId}");
            return Ok("Film wypożyczony!");
        }

        //Zwróć - prawdę mówiąc nie wiem czy będzie potrzebne, ale wrzucam na wszelki
        [ActionName("Return")]
        [HttpPost("{id}")]
        [Authorize(Policy = "ClientOnly")]
        public async Task<ActionResult> ReturnMovie(int id)
        {
            var clientIdClaim = User.FindFirst("clientId")?.Value;
            if (string.IsNullOrEmpty(clientIdClaim))
                return BadRequest("ID klienta nie istnieje");

            var clientId = int.Parse(clientIdClaim);

            var rental = await _db.Rentals
                .FirstOrDefaultAsync(r => r.ClientID == clientId &&
                                         r.MovieID == id &&
                                         r.ReturnDate == null);

            if (rental == null)
                return NotFound("Ten film nie jest wypożyczony");

            rental.ReturnDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Film zwrócony: Movie ID {id} przez Client ID: {clientId}");
            return Ok("Film zwrócony");
        }

        //Oceń film
        [ActionName("Rate")]
        [HttpPost("{id}")]
        [Authorize(Policy = "ClientOnly")]
        public async Task<ActionResult> RateMovie(int id, [FromBody] RateMovieDto ratingDto)
        {
            var clientIdClaim = User.FindFirst("clientId")?.Value;
            if (string.IsNullOrEmpty(clientIdClaim))
                return BadRequest("Klient nie istnieje");

            var clientId = int.Parse(clientIdClaim);

            // Sprawdź czy klient już kiedyś wypożyczył
            var hasRented = await _db.Rentals
                .AnyAsync(r => r.ClientID == clientId && r.MovieID == id);

            //Ta reguła może nie być konieczna, ale jeśli tylko wypożyczamy, to można zostawić
            if (!hasRented)
                return BadRequest("Możesz ocenić tylko te filmy, które wypożyczysz");

            // Sprawdź czy istnieje rating
            var existingRating = await _db.Ratings
                .FirstOrDefaultAsync(r => r.ClientID == clientId && r.MovieID == id);

            if (existingRating != null)
            {
                // ... a jeśli istnieje to update
                existingRating.Value = ratingDto.Rating;
                existingRating.Comment = ratingDto.Comment;
                existingRating.RatedAt = DateTime.UtcNow;
            }
            else
            {
                // Utwórz nowy rating
                var rating = new Rating
                {
                    ClientID = clientId,
                    MovieID = id,
                    Value = ratingDto.Rating,
                    Comment = ratingDto.Comment,
                    RatedAt = DateTime.UtcNow
                };
                _db.Ratings.Add(rating);
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation($"Film oceniony: ID filmu {id} przez klienta: {clientId}, Rating: {ratingDto.Rating}");
            return Ok("Ocena wysłana poprawnie");
        }

        //Moje wypożyczenia
        [ActionName("MyRentals")]
        [HttpGet]
        [Authorize(Policy = "ClientOnly")]
        public async Task<ActionResult<List<RentalDto>>> GetMyRentals()
        {
            var clientIdClaim = User.FindFirst("clientId")?.Value;
            if (string.IsNullOrEmpty(clientIdClaim))
                return BadRequest("Klient nie istnieje");

            var clientId = int.Parse(clientIdClaim);

            var rentals = await _db.Rentals
                .Include(r => r.Movie)
                .Where(r => r.ClientID == clientId)
                .OrderByDescending(r => r.RentalDate)
                .Select(r => new RentalDto
                {
                    Id = r.ID,
                    MovieId = r.MovieID,
                    MovieName = r.Movie.Name,
                    MovieType = r.Movie.Type,
                    RentalDate = r.RentalDate,
                    ReturnDate = r.ReturnDate,
                    IsActive = r.ReturnDate == null
                })
                .ToListAsync();

            return Ok(rentals);
        }
    }

}

