using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace B_G2_CarritoCompras.Models
{
    public class Compra
    {
        public int CompraId { get; set; }

        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }

        public DateTime Fecha { get; set;  }

        // Las compras tienen un estado, que por default es pendiente, hasta que el usuario la finalice.
        [EnumDataType(typeof(EstadoCompra))]
        public EstadoCompra Estado { get; set; } = EstadoCompra.Pendiente;

        [Display(Name = "Precio Final")]
        public double PrecioFinal { get; set; }

        [Required]
        public int CarritoId { get; set; }

        public Carrito Carrito { get; set; }
    }

    public enum EstadoCompra
    {
        Pendiente,
        Finalizada,
        Cancelada,
    }
}
