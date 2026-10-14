using CatalogoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {}

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
            .HasIndex(e => e.Email)
            .IsUnique();

            modelBuilder.Entity<Categoria>()
            .HasIndex(e => e.Nombre)
            .IsUnique();

            modelBuilder.Entity<Producto>()
            .HasIndex(e => e.ExternalId)
            .IsUnique();

            modelBuilder.Entity<Producto>()
            .Property(e => e.Precio)
            .HasPrecision(18, 2);

            modelBuilder.Entity<Categoria>()
            .HasMany<Producto>(u => u.Productos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(p => p.CategoriaId);
        }
    }
}