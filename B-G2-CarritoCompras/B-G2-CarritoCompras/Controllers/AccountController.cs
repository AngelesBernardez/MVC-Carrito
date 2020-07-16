using System.Threading.Tasks;
using B_G2_CarritoCompras.Data;
using B_G2_CarritoCompras.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EjEntityFramework.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ApplicationDbContext _contexto;

        public AccountController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            ApplicationDbContext contexto)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._contexto = contexto;
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(RegistroUsuario modelo)
        {
            if (ModelState.IsValid)
            {
                var usuario = new Usuario
                {
                    UserName = modelo.Email,
                    Email = modelo.Email,
                    Nombre = modelo.Nombre,
                    Apellido = modelo.Apellido,
                };

                var resultadoRegistracion = await _userManager.CreateAsync(usuario, modelo.Password);

                if (resultadoRegistracion.Succeeded)
                {
                    var carrito = new Carrito() { UsuarioId = usuario.Id, Activo = true };
                    _contexto.Carritos.Add(carrito);
                    _contexto.SaveChanges();

                    await _userManager.AddToRoleAsync(usuario, "Cliente");

                    await _signInManager.SignInAsync(usuario, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in resultadoRegistracion.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> EmailUsado(string email)
        {
            var usuarioExistente = await _userManager.FindByEmailAsync(email);

            if (usuarioExistente == null)
            {
                // No hay un usuario existente con ese email
                return Json(true);
            }
            else
            {
                // El mail ya está en uso
                return Json($"El correo {email} ya está en uso.");
            }

            // Utilizo JSON, Jquery Validate method, espera una respuesta de este tipo.
            // Para que esto funcione desde luego, tienen que estar como siempre las librerias de Jquery disponibles.
            // Importante, que estén en el siguiente ORDEN!!!!!
            // jquery.js
            // jquery.validate.js
            // jquery.validate.unobtrisive.js

            // Jquery está en el Layout, y luego las otras dos, están definidas en el archivo _ValidationScriptsPartial.cshtml.
            // Si incluyen el render de la sección de script esa, estará entonces disponible.
        }

        [HttpPost]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult IniciarSesion(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(Login modelo)
        {
            string returnUrl = TempData["returnUrl"] as string;

            if (ModelState.IsValid)
            {
                var resultadoInicioSesion = await _signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recordarme, false);

                if (resultadoInicioSesion.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Inicio de sesión inválido.");
            }

            return View(modelo);
        }

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AsignarRol()
        {
            // Obtener Roles.
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Ejemplo()
        {
            return View();
        }
    }
}
