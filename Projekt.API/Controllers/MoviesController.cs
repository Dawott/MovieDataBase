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

namespace Projekt.API.Controllers
{
    [Route("api/[Controller]/[action]")]
    [ApiController]
    public class MoviesController(MoviesDBContext context) : ControllerBase
    {
        private readonly MoviesDBContext _db = context;

        // GET: api/Movies/list
        [ActionName("List")]
        [HttpGet]
        public async Task<ActionResult<List<Movie>>> GetListAsync()
        {
            try
            {
                await Task.Delay(3000);
                return Ok(await _db.Movies.ToListAsync());
            }
            catch (Exception ex)
            {
                var exception = ex;
                while (exception.InnerException != null)
                    exception = exception.InnerException;

                return new ObjectResult(exception.Message) { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }
        // GET: api/Movies/ByID/1
        [ActionName("ByID")]
        [HttpGet] //("{id:range(1,200)}") - range niepotrzebny
        public Movie? GetByID(int id)
        {
            var movie = _db.Movies.Find(id);

            if (movie == null)
                throw new Exception($"Movies with id {id} does not exist!");


            return movie;
        }




        // POST: api/Movies/Add
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ActionName("Add")]
        [HttpPost]
        public async Task<ActionResult<int>> PostAdd(Movie newMovie)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var MovieDB = await _db.Movies.FirstOrDefaultAsync(m => m.Name == newMovie.Name);
            if (MovieDB != null)
                return Conflict($"Movie with name {newMovie.Name} already exist");

            _db.Movies.Add(newMovie);
            await _db.SaveChangesAsync();
            return Ok(newMovie.ID);

        }

        // DELETE: api/Movies/5
        [ActionName("Delete")]
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var movie = _db.Movies.Find(id);
            if (movie == null)
                return Conflict($"Movie with id {id} does not exist!");

            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //api/Movie/Update
        [ActionName("Update")]
        [HttpPut]
        public async Task<ActionResult> PutUpdate(Movie updatedMovie)
        {
            var movie = await _db.Movies.FindAsync(updatedMovie.ID);
            if (movie == null)
                return Conflict($"Movie {updatedMovie.Name} does not exist!");

            // Aktualizuj WSZYSTKIE pola
            movie.Name = updatedMovie.Name;
            movie.Type = updatedMovie.Type;
            movie.Rating = updatedMovie.Rating;
            movie.ClientID = updatedMovie.ClientID;

            await _db.SaveChangesAsync();
            return Ok();
        }

        private bool MovieExists(int id)
        {
            return _db.Movies.Any(e => e.ID == id);
        }

        [ActionName("UploadCover")]
        [HttpPost("{id}")]
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
    }

}

