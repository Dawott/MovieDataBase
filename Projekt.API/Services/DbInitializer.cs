using Projekt.API.Model;

namespace Projekt.API.Services
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MoviesDBContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            // Sprawdź czy baza istnieje
            await context.Database.EnsureCreatedAsync();

            // Sprawdź czy mamy dane
            if (context.Users.Any())
                return; // Baza zseedowana

            // Twórz userów - TO ZŁY POMYsL, ale do dema wystarczy
            var users = new[]
            {
                new User
                {
                    Email = "jan.kowalski@example.com",
                    PasswordHash = passwordHasher.HashPassword("jan123"),
                    Role = UserRole.Client,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "anna.nowak@example.com",
                    PasswordHash = passwordHasher.HashPassword("anna123"),
                    Role = UserRole.Client,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "admin@example.com",
                    PasswordHash = passwordHasher.HashPassword("admin123"),
                    Role = UserRole.Administrator,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // Utwórz nie-adminów
            var clients = new[]
            {
                new Client { Name = "Jan", Lastname = "Kowalski", UserID = users[0].ID },
                new Client { Name = "Anna", Lastname = "Nowak", UserID = users[1].ID }
            };

            context.Clients.AddRange(clients);
            await context.SaveChangesAsync();

            // Utwórz filmy
            var movies = new[]
            {
                new Movie { Name = "Harry Potter", Type = "Fantasy", Rating = 7, IsAvailable = true },
                new Movie { Name = "Skazany na Shawshank", Type = "Dramat", Rating = 8.7, IsAvailable = true },
                new Movie { Name = "Coco", Type = "Familijny", Rating = 7.5, IsAvailable = true }
            };

            context.Movies.AddRange(movies);
            await context.SaveChangesAsync();
        }
    }
}
