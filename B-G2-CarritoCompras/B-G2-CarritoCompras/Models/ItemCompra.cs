using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace B_G2_CarritoCompras.Models
{
    public class ItemCompra
    {
        public int ItemCompraId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        public Producto Producto { get; set; }

        [Range(1, 9999999999999999, ErrorMessage = "Debe elegir por lo menos un item.")]
        public int Cantidad { get; set; }

        public int CarritoId { get; set; }

        public Carrito Carrito { get; set; }

        [NotMapped]
        public double Subtotal
        {
            get
            {
                if (Producto != null)
                {
                    return Producto.Precio * Cantidad;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
