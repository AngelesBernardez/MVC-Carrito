using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using B_G2_CarritoCompras.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace B_G2_CarritoCompras.Models
{
    public class RolesController : Controller
    {
        private readonly SignInManager<Usuario> _signinManager;
        private readonly UserManager<Usuario> _userManager;

        public RolesController(RoleManager<UsuarioRol> roleManager, SignInManager<Usuario> signinManager, UserManager<Usuario> userManager)
        {
            _roleManager = roleManager;
            this._signinManager = signinManager;
            this._userManager = userManager;
        }

        public RoleManager<UsuarioRol> _roleManager { get; }

        [HttpGet]
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Crear(CrearRol model)
        {
            if (ModelState.IsValid)
            {
                UsuarioRol identityRole = new UsuarioRol
                {
                    Name = model.RoleName,
                };

                IdentityResult resultado = await _roleManager.CreateAsync(identityRole);

                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Roles");
                }

                foreach (IdentityError identityError in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, identityError.Description);
                }
            }

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRolesUsuario()
        {
            if (_signinManager.IsSignedIn(User))
            {
                var roles = await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync(User.Identity.Name));
                return View(roles);
            }

            return View(new List<string>());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Editar(string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);

            if (rol == null)
            {
                ViewBag.ErrorMessage = $"El rol con el Id = {id} no existe";
                return View("NotFound");
            }

            var modeloEditRol = new EditarRol
            {
                Id = rol.Id,
                RoleName = rol.Name,
            };
            try
            {
                foreach (var usr in _userManager.Users)
                {
                    if (await _userManager.IsInRoleAsync(usr, rol.Name))
                    {
                        modeloEditRol.Usuarios.Add(usr.UserName);
                    }
                }
            }
            catch (SqlNullValueException)
            {
                // No es necesario
            }

            return View(modeloEditRol);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Editar(EditarRol model)
        {
            var rolBuscado = await _roleManager.FindByIdAsync(model.Id.ToString());

            if (rolBuscado == null)
            {
                ViewBag.ErrorMessage = $"El rol con el Id = {model.Id} no existe";
                return View("NotFound");
            }
            else
            {
                rolBuscado.Name = model.RoleName;
                var resultado = await _roleManager.UpdateAsync(rolBuscado);

                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarUsuariosConRol(string rolId)
        {
            ViewBag.rolId = rolId;

            var rolBuscado = await _roleManager.FindByIdAsync(rolId);

            if (rolBuscado == null)
            {
                ViewBag.ErrorMessage = $"El rol con el Id = {rolId} no existe";
                return View("NotFound");
            }

            var model = new List<UsuarioRol>();

            foreach (var user in _userManager.Users)
            {
                var usuarioRol = new UsuarioRol
                {
                    UsuarioId = user.Id,
                    NombreUsuario = user.UserName,
                };

                if (await _userManager.IsInRoleAsync(user, rolBuscado.Name))
                {
                    usuarioRol.Seleccionado = true;
                }
                else
                {
                    usuarioRol.Seleccionado = false;
                }

                model.Add(usuarioRol);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarUsuariosConRol(List<UsuarioRol> model, string rolId)
        {
            var rolBuscado = await _roleManager.FindByIdAsync(rolId);

            if (rolBuscado == null)
            {
                ViewBag.ErrorMessage = $"El rol con el Id = {rolId} no existe";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UsuarioId.ToString());

                IdentityResult resultado = null;

                if (model[i].Seleccionado && !(await _userManager.IsInRoleAsync(user, rolBuscado.Name)))
                {
                    resultado = await _userManager.AddToRoleAsync(user, rolBuscado.Name);
                }
                else if (!model[i].Seleccionado && await _userManager.IsInRoleAsync(user, rolBuscado.Name))
                {
                    resultado = await _userManager.RemoveFromRoleAsync(user, rolBuscado.Name);
                }
                else
                {
                    continue;
                }

                if (resultado.Succeeded)
                {
                    // valido si me faltan pasadas
                    if (i < model.Count - 1)
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("Editar", new { Id = rolId });
                    }
                }
            }

            return RedirectToAction("Editar", new { Id = rolId });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);

            if (rol == null)
            {
                ViewBag.ErrorMessage = $"El rol con el Id = {id} no existe";
                return View("NotFound");
            }

            return View(rol);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(IdentityRole model)
        {
            var rol = await _roleManager.FindByIdAsync(model.Id);

            if (rol == null)
            {
                ViewBag.ErrorMessage = $"El rol con el Id = {model.Id} no existe";
                return View("NotFound");
            }

            var resultado = await _roleManager.DeleteAsync(rol);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var er in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, er.Description);
            }

            return View(model);
        }
    }
}