namespace Projekt.API.Utilities
{
    //Istnieje tylko dla SeedData
    public class SeedDataHelper
    {
        public static string GetHashedPassword(string plainPassword)
        {
            var hasher = new Services.PasswordHasher();
            return hasher.HashPassword(plainPassword);
        }
    }
}
