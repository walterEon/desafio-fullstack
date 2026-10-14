using CatalogoApi.Data;
using CatalogoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.Data;
public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, IConfiguration config)
    {
        var email = config["Seed:Email"];
        var password = config["Seed:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Faltan Seed:Email o Seed:Password en la configuración.");

        if (await db.Usuarios.AnyAsync(e => e.Email == email))
            return;

        db.Usuarios.Add(new Usuario { Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(password) });
        await db.SaveChangesAsync();
    }
}