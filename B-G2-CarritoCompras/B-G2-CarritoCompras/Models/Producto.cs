using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B_G2_CarritoCompras.Models
{
    public class Producto
    {
        public const string MensajeCampoRequerido = "El campo es requerido.";

        public int ProductoId { get; set; }

        [Required(ErrorMessage = MensajeCampoRequerido)]
        public int CategoriaId { get; set; }

        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = MensajeCampoRequerido)]
        [StringLength(50, ErrorMessage = "El atributo {0} debe tener un máximo de {1} y un mínimo de {2}.", MinimumLength = 5)]
        [Display(Name = "Producto")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = MensajeCampoRequerido)]
        [Range(0, 9999999999999999.99, ErrorMessage = "El atributo {0} debe ser positivo.")]
        public double Precio { get; set; }

        [Required(ErrorMessage = MensajeCampoRequerido)]
        [Range(0, 9999999999999999, ErrorMessage = "El atributo {0} debe ser positivo.")]
        public int Stock { get; set; }

        // Propiedades Navegacionales.
        public List<Carrito> Carritos { get; set; }

        [NotMapped]
        public List<Compra> Compras { get; set; }
    }
}
