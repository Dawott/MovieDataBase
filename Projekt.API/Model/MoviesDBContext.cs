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
        public DbSet<User> Users { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Rating> Ratings
        {
            get; set;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User-Client relacja 1-1
            modelBuilder.Entity<User>()
                .HasOne(u => u.Client)
                .WithOne(c => c.User)
                .HasForeignKey<Client>(c => c.UserID);

            // Relacja wypożyczenia
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Client)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.ClientID);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Rentals)
                .HasForeignKey(r => r.MovieID);

            // Relacja na ratingi
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Client)
                .WithMany(c => c.Ratings)
                .HasForeignKey(r => r.ClientID);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Ratings)
                .HasForeignKey(r => r.MovieID);

            // Constraint: Rating-Film-Klient
            modelBuilder.Entity<Rating>()
                .HasIndex(r => new { r.ClientID, r.MovieID })
                .IsUnique();

            // Dane Seed już zhashowane
            var hasher = new Services.PasswordHasher();

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    ID = 1,
                    Email = "jan.kowalski@example.com",
                    PasswordHash = hasher.HashPassword("jan123"), // Nie muszę chyba tłumaczyć, że to głupi pomysł, ale... do dema
                    Role = UserRole.Client,
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new User
                {
                    ID = 2,
                    Email = "anna.nowak@example.com",
                    PasswordHash = hasher.HashPassword("anna123"),
                    Role = UserRole.Client,
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new User
                {
                    ID = 3,
                    Email = "admin@example.com",
                    PasswordHash = hasher.HashPassword("admin123"),
                    Role = UserRole.Administrator,
                    CreatedAt = new DateTime(2025, 1, 1)
                }
            );

            modelBuilder.Entity<Client>().HasData(
                new Client { ID = 1, Name = "Jan", Lastname = "Kowalski", UserID = 1 },
                new Client { ID = 2, Name = "Anna", Lastname = "Nowak", UserID = 2 });

            modelBuilder.Entity<Movie>().HasData(
                new Movie { ID = 1, Name = "Harry Potter", Type = "Fantasy", Rating = 7, IsAvailable = true },
                new Movie { ID = 2, Name = "Skazany na Shawshank", Type = "Dramat", Rating = 8.7, IsAvailable = true },
                new Movie { ID = 3, Name = "Coco", Type = "Familijny", Rating = 7.5, IsAvailable = true });


        }
    }
}
