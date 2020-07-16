using System.Linq;
using System.Threading.Tasks;
using B_G2_CarritoCompras.Data;
using B_G2_CarritoCompras.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace B_G2_CarritoCompras.Controllers
{
    public class CarritosController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ApplicationDbContext _context;

        public CarritosController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Carritos
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var carrito = await GetCarritoActivoDeUsuario();

            return View(carrito);
        }

        // GET: Carritos
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexAdmin()
        {
            var carritos = _context.Carritos
                .Include(c => c.ItemsCompra)
                .Include(c => c.Usuario);

            return View(await carritos.ToListAsync());
        }

         // GET: Carritos/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CarritoId == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // GET: Carritos/Create
        [Authorize]
        public IActionResult Create()
        {
            var usrs = _context.Usuarios;

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido");

            return View();
        }

        // POST: Carritos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarritoId,UsuarioId")] Carrito carrito)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carrito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellido", carrito.UsuarioId);
            return View(carrito);
        }

        // GET: Carritos/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos.FindAsync(id);
            if (carrito == null)
            {
                return NotFound();
            }

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellido", carrito.UsuarioId);
            return View(carrito);
        }

        // POST: Carritos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarritoId,UsuarioId")] Carrito carrito)
        {
            if (id != carrito.CarritoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carrito.CarritoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellido", carrito.UsuarioId);
            return View(carrito);
        }

        // GET: Carritos/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CarritoId == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carrito = await _context.Carritos.FindAsync(id);
            _context.Carritos.Remove(carrito);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<Carrito> GetCarritoActivoDeUsuario()
        {
            var usuario = await _userManager.FindByEmailAsync(User.Identity.Name);

            // Devuelve el carrito del usuario logueado.
            var carrito = _context.Carritos
               .Include(c => c.ItemsCompra).ThenInclude(ic => ic.Producto).ThenInclude(p => p.Categoria)
               .Include(c => c.Usuario)
               .Where(c => c.Activo == true)
               .FirstOrDefault(cu => cu.UsuarioId == usuario.Id);

            return carrito;
        }

        public async Task<IActionResult> QuitarItemDelCarrito(int id)
        {
            var carrito = await GetCarritoActivoDeUsuario();

            if (carrito == null) return NotFound();

            var item = carrito.ItemsCompra.Single(i => i.ItemCompraId == id);

            if (item == null) return NotFound();

            _context.ItemsCompras.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Carritos");
        }

        public async Task<IActionResult> VaciarCarrito()
        {
            var carrito = await GetCarritoActivoDeUsuario();

            if (carrito == null) return NotFound();

            _context.ItemsCompras.RemoveRange(carrito.ItemsCompra);
            _context.SaveChanges();

            return RedirectToAction("Index", "Carritos");
        }

        private bool CarritoExists(int id)
        {
            return _context.Carritos.Any(e => e.CarritoId == id);
        }
    }
}
