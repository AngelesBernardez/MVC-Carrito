using System.Collections.Generic;
using B_G2_CarritoCompras.Models;

namespace B_G2_CarritoCompras.ViewModels
{
    public class ProductosVM
    {
        public List<Producto> Productos { get; set; }

        public int? CategoriaId { get; set; } = null;

        public Categoria Categoria { get; set; }
    }
}
