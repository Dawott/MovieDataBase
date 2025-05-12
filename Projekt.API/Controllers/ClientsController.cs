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
    [Route("api/[controller]/[action]")]
    [ApiController]
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
        [HttpGet("{id:range(1,200)}")]
        public Client? GetByID(int id)
        {
            var client =  _db.Clients.Find(id);

            if (client == null)
                throw new Exception($"Client with id {id} does not exist!");
            

            return client;
        }



        

        // POST: api/Clients/Add
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ActionName("Add")]
        [HttpPost]
        public async Task<ActionResult<int>> PostAdd([FromBody]Client newClient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var clientDB = await _db.Clients.FirstOrDefaultAsync(c=> c.Lastname == newClient.Lastname);
            if (clientDB != null)
                return Conflict($"Client with lastname {newClient.Lastname} already exist");

            _db.Clients.Add(newClient);
            await _db.SaveChangesAsync();
            return Ok(newClient.ID);
     
        }

        // DELETE: api/Clients/5
        [ActionName("Delete")]
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var client =  _db.Clients.Find(id);
            if (client == null)
                return Conflict($"Client with id {id} does not exist!");

            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //api/Client/Update
        [ActionName("Update")]
        [HttpPatch]
        public async Task<ActionResult> PatchUpdate(Client updatedClient)
        {
            var client = await _db.Clients.FindAsync(updatedClient.ID);
            if (client == null)
                return Conflict($"Client {updatedClient.Lastname} does not exist!");

            // Aktualizuj WSZYSTKIE pola
            client.Name = updatedClient.Name;
            client.Lastname = updatedClient.Lastname;

            await _db.SaveChangesAsync();
            return Ok();
        }

        private bool ClientExists(int id)
        {
            return _db.Clients.Any(e => e.ID == id);
        }
    }
}
