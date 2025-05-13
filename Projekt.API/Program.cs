using Microsoft.EntityFrameworkCore;
using Projekt.API.Model;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var moviesConnectionsString = builder.Configuration.GetConnectionString("MoviesDB");          
builder.Services.AddDbContext<MoviesDBContext>(options =>options.UseSqlServer(moviesConnectionsString));    



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Dodano serwis Serilog
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

//CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader()); // Zezwól na wszystkie nag³ówki, w tym Content-Type
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/Uploads"
});

//CORS  
app.UseCors("AllowAll");
app.UseSerilogRequestLogging();
app.UseAuthorization();
app.MapControllers();
app.Run();
