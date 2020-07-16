using System;
using System.ComponentModel.DataAnnotations;

namespace B_G2_CarritoCompras.Models
{
    public class RegistroUsuario
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmación de Password")]
        [Compare("Password", ErrorMessage = "La contraseña debe ser igual a la ingresada previamente.")]
        public string ConfirmacionPassword { get; set; }
    }
}
