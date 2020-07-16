using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace B_G2_CarritoCompras.Models.Roles
{
    public class EditarRol
    {
        public int Id { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "El {0} es requerido")]
        public string RoleName { get; set; }

        public List<string> Usuarios { get; set; } = new List<string>();
    }
}
