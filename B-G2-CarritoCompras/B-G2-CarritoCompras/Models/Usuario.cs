using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace B_G2_CarritoCompras.Models
{
    public class Usuario : IdentityUser<int>
    {
        public const string MensajeCampoRequerido = "El campo es requerido.";

        [Required(ErrorMessage = MensajeCampoRequerido)]
        [StringLength(50, ErrorMessage = "El atributo {0} debe tener un máximo de {1} y un mínimo de {2}.", MinimumLength = 2)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = MensajeCampoRequerido)]
        [StringLength(50, ErrorMessage = "El atributo {0} debe tener un máximo de {1} y un mínimo de {2}.", MinimumLength = 2)]
        public string Apellido { get; set; }

        public ICollection<Carrito> Carritos { get; set; }

        // Propiedad Navegacional
        [Display(Name="Compras Realizadas")]
        public List<Compra> ComprasRealizadas { get; set; }

        public DateTime FechaDeAlta { get; } = DateTime.Now;
    }
}
