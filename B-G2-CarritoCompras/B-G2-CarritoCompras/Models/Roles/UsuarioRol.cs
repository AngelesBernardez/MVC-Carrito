using Microsoft.AspNetCore.Identity;

namespace B_G2_CarritoCompras.Models.Roles
{
    public class UsuarioRol : IdentityRole<int>
    {
        public int UsuarioId { get; set; }

        public int RolId { get; set; }

        public string NombreUsuario { get; set; }

        public bool Seleccionado { get; set; }
    }
}
