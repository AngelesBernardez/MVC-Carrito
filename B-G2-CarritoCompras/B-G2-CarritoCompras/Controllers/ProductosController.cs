using System.Linq;
using System.Threading.Tasks;
using B_G2_CarritoCompras.Data;
using B_G2_CarritoCompras.Models;
using B_G2_CarritoCompras.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace B_G2_CarritoCompras.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ProductosController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Productos
        public async Task<IActionResult> Index(ProductosVM productosVM, int? orden)
        {
            if (productosVM.CategoriaId != null)
            {
                var productos = await _context.Productos
                    .Include(p => p.Categoria)
                    .Where(p => p.CategoriaId == productosVM.CategoriaId).ToListAsync();

                var categoria = await _context.Categorias
                    .FirstOrDefaultAsync(c => c.CategoriaId == productosVM.CategoriaId);

                productosVM.Productos = productos;
                productosVM.Categoria = categoria;

                ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre", productosVM.CategoriaId);
            }
            else
            {
                var productos = _context.Productos.Include(p => p.Categoria).ToList();
                productosVM.Productos = productos;
                ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre");
            }

            // Sé que no es lo mejor.
            if (orden == 1) productosVM.Productos = productosVM.Productos.OrderByDescending(p => p.Precio).ToList();
            if (orden == 2) productosVM.Productos = productosVM.Productos.OrderBy(p => p.Precio).ToList();
            if (orden == 3) productosVM.Productos = productosVM.Productos.OrderBy(p => p.Nombre).ToList();
            if (orden == 4) productosVM.Productos = productosVM.Productos.OrderByDescending(p => p.Nombre).ToList();

            return View(productosVM);
        }

        // GET: Productos/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int? categoriaId)
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre", categoriaId);
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductoId,CategoriaId,Nombre,Precio,Stock")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoId,CategoriaId,Nombre,Precio,Stock")] Producto producto)
        {
            if (id != producto.ProductoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.ProductoId))
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

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> AddToCartAsync(int id)
        {
            var usuario = await _userManager.FindByEmailAsync(User.Identity.Name);

            // Devuelve el carrito del usuario logueado.
            var carrito = _context.Carritos
               .Include(c => c.ItemsCompra).ThenInclude(ic => ic.Producto)
               .Where(c => c.Activo == true)
               .FirstOrDefault(cu => cu.UsuarioId == usuario.Id);

            // Al agregar un producto al carrito, chequeo que el usuario ya no tenga
            // el itemCompra que corresponda a ese producto.
            if (carrito.ItemsCompra.Count > 0)
            {
                foreach (ItemCompra item in carrito.ItemsCompra)
                {
                    if (item.ProductoId == id)
                    {
                        return RedirectToAction("Edit", "ItemCompras", new { id = item.ItemCompraId });
                    }
                }
            }

            return RedirectToAction("Create", "ItemCompras", new { id });
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoId == id);
        }
    }
}
