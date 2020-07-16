using System.Linq;
using System.Threading.Tasks;
using B_G2_CarritoCompras.Data;
using B_G2_CarritoCompras.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace B_G2_CarritoCompras.ViewComponents
{
    public class CarritoViewComponent : ViewComponent
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ApplicationDbContext _context;

        public CarritoViewComponent(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var usuario = await _userManager.FindByEmailAsync(User.Identity.Name);

            var carrito = await _context.Carritos
               .Include(c => c.ItemsCompra)
               .Where(c => c.Activo == true)
               .FirstOrDefaultAsync(cu => cu.UsuarioId == usuario.Id);

            return View(carrito);
        }
    }
}
