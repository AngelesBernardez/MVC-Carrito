using System.ComponentModel.DataAnnotations;

namespace B_G2_CarritoCompras.Models.Roles
{
    public class CrearRol
    {
        [Required]
        public string RoleName { get; set; }
    }
}
