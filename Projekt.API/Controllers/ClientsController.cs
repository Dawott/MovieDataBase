using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.API.Model;

namespace Projekt.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly MoviesDBContext _db;

        public ClientsController(MoviesDBContext context)
        {
            _db = context;
        }

        // GET: api/Clients/list
        [ActionName("List")]
        [HttpGet]
        [Authorize(Policy ="AdminOnly")] //Tylko admini mogą widzieć całą listę
        public async Task<ActionResult<List<Client>>> GetListAsync()
        {
            try
            {
                await Task.Delay(3000);
                return Ok(await _db.Clients.ToListAsync());
            }
            catch (Exception ex)
            {
                var exception = ex;
                while (exception.InnerException != null)
                    exception = exception.InnerException;

                return new ObjectResult(exception.Message) { StatusCode=(int)HttpStatusCode.InternalServerError}; 
            }
        }
        // GET: api/Clients/ByID/1
        [ActionName("ByID")]
        [HttpGet]
        public async Task<ActionResult<Client>> GetByID(int id) //Zmiana na Async
        {
            //var client =  _db.Clients.Find(id);
            //Sprawdzanie czy user jest adminem czy jednak sprawdza własny profil
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var clientId = User.FindFirst("clientId")?.Value;

            if (userRole != "Administrator" && (string.IsNullOrEmpty(clientId) || clientId != id.ToString()))
                return Forbid("Możesz zobaczyć tylko własny profil");

            var client = await _db.Clients.Include(c => c.User).FirstOrDefaultAsync(c => c.ID == id);

            if (client == null)
                throw new Exception($"Klient z danym id {id} nie istnieje!");
            

            return client;
        }

        // POST: api/Clients/Add
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ActionName("Add")]
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<int>> PostAdd([FromBody]Client newClient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           /* var clientDB = await _db.Clients.FirstOrDefaultAsync(c=> c.Lastname == newClient.Lastname);
            if (clientDB != null)
                return Conflict($"Klient z nazwiskiem {newClient.Lastname} already exist");*/ //Na razie komentuję, bo mogą być klienci z tym samym nazwiskiem

            _db.Clients.Add(newClient);
            await _db.SaveChangesAsync();
            return Ok(newClient.ID);
     
        }

        // DELETE: api/Clients/5
        [ActionName("Delete")]
        [HttpDelete]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Delete(int id)
        {
            var client =  _db.Clients.Find(id);
            if (client == null)
                return Conflict($"Klient z danym id {id} nie istnieje!");

            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //api/Client/Update
        [ActionName("Update")]
        [HttpPatch]
        public async Task<ActionResult> PatchUpdate(Client updatedClient)
        {
            //Analogicznie do get by ID
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var clientId = User.FindFirst("clientId")?.Value;

            if (userRole != "Administrator" && (string.IsNullOrEmpty(clientId) || clientId != updatedClient.ID.ToString()))
                return Forbid("You can only update your own information");

            var client = await _db.Clients.FindAsync(updatedClient.ID);
            if (client == null)
                return Conflict($"Klient z {updatedClient.ID} nie istnieje!");

            // Aktualizuj WSZYSTKIE pola
            client.Name = updatedClient.Name;
            client.Lastname = updatedClient.Lastname;

            await _db.SaveChangesAsync();
            return Ok();
        }

        //Nowy kontroler do widoku Mój profil
        [ActionName("MyProfile")]
        [HttpGet]
        [Authorize(Policy = "ClientOnly")]
        public async Task<ActionResult<Client>> GetMyProfile()
        {
            var clientId = User.FindFirst("clientId")?.Value;
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("Nie odnaleziono profilu dla tego klienta");

            var client = await _db.Clients
                .Include(c => c.User)
                .Include(c => c.Rentals)
                    .ThenInclude(r => r.Movie)
                .Include(c => c.Ratings)
                    .ThenInclude(r => r.Movie)
                .FirstOrDefaultAsync(c => c.ID == int.Parse(clientId));

            if (client == null)
                return NotFound();

            return Ok(client);
        }

        private bool ClientExists(int id)
        {
            return _db.Clients.Any(e => e.ID == id);
        }
    }
}
