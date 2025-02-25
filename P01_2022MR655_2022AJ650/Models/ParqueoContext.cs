using Microsoft.EntityFrameworkCore;

namespace P01_2022MR655_2022AJ650.Models
{
    public class ParqueoContext : DbContext
    {
        public ParqueoContext(DbContextOptions<ParqueoContext> options)
           : base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Sucursales> Sucursales { get; set; }

        public DbSet<EspaciosParqueo> EspaciosParqueo { get; set; }


        public DbSet<Reservas> Reservas { get; set; }

        public ParqueoContext() { }
    }
}
