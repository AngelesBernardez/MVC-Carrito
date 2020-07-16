using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B_G2_CarritoCompras.Models
{
    public class Carrito
    {
        public int CarritoId { get; set; }

        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }

        public bool Activo { get; set; } = false;

        // Propiedades Navegacionales.
        [Display(Name = "Items en el carrito")]
        public List<ItemCompra> ItemsCompra { get; set; } = new List<ItemCompra>();

        private double _subtotal;

        [NotMapped]
        public double Subtotal
        {
            get
            {
                _subtotal = 0;
                foreach (ItemCompra item in ItemsCompra)
                {
                   _subtotal += item.Subtotal;
                }

                return _subtotal;
            }
        }
    }
}
