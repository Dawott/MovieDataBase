using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.API.Model;
using Projekt.API.Services;
using System.Text.RegularExpressions;
using static Projekt.API.DTOs.AuthDTO;

namespace Projekt.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MoviesDBContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            MoviesDBContext context,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _db = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _logger = logger;
        }

        [ActionName("Register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Sprawdzanie inputu
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Walidacja dodatkowa na email
                if (!IsValidEmail(registerDto.Email))
                    return BadRequest("Błędny format");

                // Sprawdzenie czy użytkownik istnieje
                var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
                if (existingUser != null)
                    return Conflict("Wygląda na to, że masz tu już konto");

                // Utwórz nowego użytkownika
                var user = new User
                {
                    Email = registerDto.Email,
                    PasswordHash = _passwordHasher.HashPassword(registerDto.Password),
                    Role = UserRole.Client,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                // Dopisz klienta do użytkownika
                var client = new Client
                {
                    Name = registerDto.Name,
                    Lastname = registerDto.Lastname,
                    UserID = user.ID
                };

                _db.Clients.Add(client);
                await _db.SaveChangesAsync();

                // Generuj Token
                user.Client = client;
                var token = _jwtService.GenerateToken(user);

                _logger.LogInformation($"Zarejestrowano: {user.Email}");

                return Ok(new LoginResponseDto
                {
                    Token = token,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    ClientId = client.ID,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60) // 60 minut jak w settingsach - WAZNE
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas rejestracji");
                return StatusCode(500, "Wystąpił błąd podczas rejestracji");
            }
        }

        [ActionName("Login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Find user by email
                var user = await _db.Users
                    .Include(u => u.Client)
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null)
                    return Unauthorized("Błędy email lub hasło");

                // Verify password
                if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
                    return Unauthorized("Błędy email lub hasło");

                // Generate JWT token
                var token = _jwtService.GenerateToken(user);

                _logger.LogInformation($"Użytkownik zalogowany jako: {user.Email}");

                return Ok(new LoginResponseDto
                {
                    Token = token,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    ClientId = user.Client?.ID,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas logowania");
                return StatusCode(500, "Natknięto się na błąd");
            }
        }

        [ActionName("Me")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _db.Users
                    .Include(u => u.Client)
                    .FirstOrDefaultAsync(u => u.ID == int.Parse(userId));

                if (user == null)
                    return NotFound();

                return Ok(new UserDto
                {
                    Id = user.ID,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Client = user.Client != null ? new ClientDto
                    {
                        Id = user.Client.ID,
                        Name = user.Client.Name,
                        Lastname = user.Client.Lastname
                    } : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wczytywania użytkownika");
                return StatusCode(500, "Wystąpił nieoczekiwany błąd");
            }
        }

        [ActionName("RegisterAdmin")]
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<LoginResponseDto>> RegisterAdmin([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Walidacja
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Sprawdzanie czy istnieje
                var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
                if (existingUser != null)
                    return Conflict("Taki admin już istnieje");

                // Utwórz nowego admina
                var user = new User
                {
                    Email = registerDto.Email,
                    PasswordHash = _passwordHasher.HashPassword(registerDto.Password),
                    Role = UserRole.Administrator,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Zarejestrowano nowego admina: {user.Email}");

                return Ok(new LoginResponseDto
                {
                    Token = _jwtService.GenerateToken(user),
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    ClientId = null,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas rejestracji nowego admina");
                return StatusCode(500, "Wystąpił błąd podczas rejestracji nowego admina");
            }
        }

        //Zmiana hasła, ale bardziej chodzi tutaj o walidację maila, którą sobie używamy w kontrolerze filmów
        [ActionName("ChangePassword")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _db.Users.FindAsync(int.Parse(userId));
                if (user == null)
                    return NotFound();

                // Weryfikacja hasła
                if (!_passwordHasher.VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                    return BadRequest("Aktualne hasło jest błędne");

                // Update hasła
                user.PasswordHash = _passwordHasher.HashPassword(changePasswordDto.NewPassword);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Hasło zmienione dla: {user.Email}");

                return Ok("Udało zmienić się hasło");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd przy zmianie hasła");
                return StatusCode(500, "Wystąpił błąd");
            }
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailRegex);
        }
    }
}