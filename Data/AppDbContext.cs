using BiblioGest.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace BiblioGest.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Livre> Livres { get; set; }
        public DbSet<Adherent> Adherents { get; set; }
        public DbSet<Emprunt> Emprunts { get; set; }
        public DbSet<Categorie> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql("Host=localhost;Port=5432;Database=LibraryDB;Username=postgres;Password=osotonase");
        }
    }
}