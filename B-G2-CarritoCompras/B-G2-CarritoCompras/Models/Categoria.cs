using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace B_G2_CarritoCompras.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public string Nombre { get; set; }

        public List<Producto> Productos { get; set; } = new List<Producto>();
    }
}
