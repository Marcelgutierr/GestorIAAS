using Microsoft.EntityFrameworkCore;
using GestorIAAS.Models;

namespace GestorIAAS.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TipoIAAS> TiposIAAS { get; set; }
        public DbSet<ServicioClinico> ServiciosClinicos { get; set; }
        public DbSet<RegistroIAAS> RegistrosIAAS { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=iaas.db");
        }
    }
}