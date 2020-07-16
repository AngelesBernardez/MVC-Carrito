using B_G2_CarritoCompras.Data;
using B_G2_CarritoCompras.Models;

namespace B_G2_CarritoCompras
{
    public static class Helpers
    {
        public const double IVA = 0.21;

        public static bool ValidarStock(Producto producto, int cantidad)
        {
            bool stockApto = false;

            if (producto != null)
            {
                if (producto.Stock >= cantidad)
                {
                    stockApto = true;
                }
            }

            return stockApto;
        }

        public static void RemoverDeStock(Producto producto, int cantidad, ApplicationDbContext _context)
        {
            producto.Stock -= cantidad;
            _context.Update(producto);
            _context.SaveChanges();
        }
    }
}
