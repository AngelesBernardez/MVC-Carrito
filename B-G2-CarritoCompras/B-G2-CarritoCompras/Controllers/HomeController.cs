using System.Diagnostics;
using System.Threading.Tasks;
using B_G2_CarritoCompras.Models;
using B_G2_CarritoCompras.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace B_G2_CarritoCompras.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoleManager<UsuarioRol> roleManager;
        private readonly UserManager<Usuario> userManager;

        public HomeController(RoleManager<UsuarioRol> roleManager, UserManager<Usuario> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            await roleManager.CreateAsync(new UsuarioRol { Name = "Admin" });
            await roleManager.CreateAsync(new UsuarioRol { Name = "Cliente" });
            await userManager.CreateAsync(
                new Usuario
            {
                Apellido = "Admin",
                Nombre = "Admin",
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
            }, "admin");

            await userManager.AddToRoleAsync(await userManager.FindByNameAsync("admin@admin.com"), "Admin");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
