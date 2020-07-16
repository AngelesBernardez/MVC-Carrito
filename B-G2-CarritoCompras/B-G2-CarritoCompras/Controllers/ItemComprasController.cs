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
    public class ItemComprasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ItemComprasController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ItemCompras
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ItemsCompras.Include(i => i.Carrito).Include(i => i.Producto);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ItemCompras/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemCompra = await _context.ItemsCompras
                .Include(i => i.Carrito)
                .Include(i => i.Producto)
                .FirstOrDefaultAsync(m => m.ItemCompraId == id);
            if (itemCompra == null)
            {
                return NotFound();
            }

            return View(itemCompra);
        }

        // GET: ItemCompras/Create
        [Authorize]
        public IActionResult Create(int? id)
        {
            var producto = _context.Productos
                .FirstOrDefault(p => p.ProductoId == id);

            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", id);
            ViewData["Stock"] = producto.Stock;

            return View();
        }

        // POST: ItemCompras/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemCompraId,ProductoId,Cantidad")] ItemCompra itemCompra)
        {
            var usuario = await _userManager.FindByEmailAsync(User.Identity.Name);
            var carrito = _context.Carritos
               .Include(c => c.Usuario)
               .Where(c => c.Activo == true)
               .FirstOrDefault(cu => cu.UsuarioId == usuario.Id);

            itemCompra.CarritoId = carrito.CarritoId;
            await _context.SaveChangesAsync();

            var producto = _context.Productos
                 .FirstOrDefault(p => itemCompra.ProductoId == p.ProductoId);

            if (ModelState.IsValid)
            {
                // Valido si hay stock
                var stock = Helpers.ValidarStock(producto, itemCompra.Cantidad);

                if (stock)
                {
                    _context.Add(itemCompra);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Carritos");
                }
                else
                {
                    ModelState.AddModelError("Cantidad", "No hay stock suficiente del producto seleccionado.");
                    ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", itemCompra.ProductoId);
                    ViewData["Stock"] = producto.Stock;
                    return View(itemCompra);
                }
            }

            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", itemCompra.ProductoId);
            return View(itemCompra);
        }

        // GET: ItemCompras/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemCompra = await _context.ItemsCompras
                .Include(ic => ic.Producto)
                .FirstOrDefaultAsync(ic => ic.ItemCompraId == id);

            if (itemCompra == null)
            {
                return NotFound();
            }

            return View(itemCompra);
        }

        // POST: ItemCompras/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemCompraId,ProductoId,Cantidad,CarritoId")] ItemCompra itemCompra)
        {
            if (id != itemCompra.ItemCompraId)
            {
                return NotFound();
            }

            // Hice esto porque itemCompra no tenia el Producto y lo necesitaba para la vista en caso de stock=false;
            var producto = _context.Productos
                .FirstOrDefault(p => itemCompra.ProductoId == p.ProductoId);
            itemCompra.Producto = producto;

            if (ModelState.IsValid)
            {
                bool stock = Helpers.ValidarStock(itemCompra.Producto, itemCompra.Cantidad);
                if (stock && itemCompra.Cantidad > 0)
                {
                    try
                    {
                        _context.Update(itemCompra);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", "Carritos");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ItemCompraExists(itemCompra.ItemCompraId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else if (itemCompra.Cantidad == 0)
                {
                    ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor a 0.");
                    ModelState.Clear();
                    return View(itemCompra);
                }
                else
                {
                    ModelState.AddModelError("Cantidad", "No hay stock suficiente del producto seleccionado.");
                    ModelState.Clear();
                    return View(itemCompra);
                }
            }

            ViewData["CarritoId"] = new SelectList(_context.Carritos, "CarritoId", "CarritoId", itemCompra.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", itemCompra.ProductoId);
            return View(itemCompra);
        }

        // GET: ItemCompras/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemCompra = await _context.ItemsCompras
                .Include(i => i.Carrito)
                .Include(i => i.Producto)
                .FirstOrDefaultAsync(m => m.ItemCompraId == id);
            if (itemCompra == null)
            {
                return NotFound();
            }

            return View(itemCompra);
        }

        // POST: ItemCompras/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemCompra = await _context.ItemsCompras.FindAsync(id);
            _context.ItemsCompras.Remove(itemCompra);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemCompraExists(int id)
        {
            return _context.ItemsCompras.Any(e => e.ItemCompraId == id);
        }
    }
}
