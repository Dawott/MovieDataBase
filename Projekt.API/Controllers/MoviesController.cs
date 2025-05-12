using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.API.Model;

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
        [HttpGet("{id:range(1,200)}")]
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
        
    }

}
