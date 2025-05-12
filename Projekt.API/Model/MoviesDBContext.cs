using Microsoft.EntityFrameworkCore;

namespace Projekt.API.Model
{
    public class MoviesDBContext : DbContext
    {
        public MoviesDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>().HasData(
                new Client { ID = 1, Name = "Jan", Lastname = "Kowalski" },
                new Client { ID = 2, Name = "Anna", Lastname = "Nowak" });

            modelBuilder.Entity<Movie>().HasData(
                new Movie { ID = 1, Name = "Harry Potter", Type = "Fantasy",ClientID=1, Rating = 7 },
                new Movie { ID = 2, Name = "Skazany na Shawshank", Type = "Dramat", ClientID=2, Rating = 8.7 },
                new Movie { ID = 3, Name = "Coco", Type = "Familijny", ClientID=1, Rating = 7.5 });


        }
    }
}
