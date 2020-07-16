using B_G2_CarritoCompras.Models;
using B_G2_CarritoCompras.Models.Roles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace B_G2_CarritoCompras.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario, UsuarioRol, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Carrito> Carritos { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Compra> Compras { get; set; }

        public DbSet<ItemCompra> ItemsCompras { get; set; }

        public DbSet<Producto> Productos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>()
                .Ignore(p => p.Compras);
        }
    }
}
