using System;
using System.Collections.Generic;
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
    public class ComprasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ComprasController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Compras
        [Authorize]
        public async Task<IActionResult> Index(int? orden) // Decidi usar un int porque lo puedo nullear, no asi el string
        {
            var usuario = await _userManager.FindByEmailAsync(User.Identity.Name);

            var compras = _context.Compras
                .Include(c => c.Carrito).ThenInclude(c => c.ItemsCompra).ThenInclude(ic => ic.Producto)
                .Where(c => c.UsuarioId == usuario.Id);

            if (orden == 1) compras = compras.OrderByDescending(c => c.Fecha); // Mas nuevas

            if (orden == 2) compras = compras.OrderBy(c => c.Fecha); // Mas antiguas

            return View(compras);
        }

        // GET: Compras
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexAdmin(int? orden)
        {
            var compras = _context.Compras
                .Include(c => c.Usuario)
                .Include(c => c.Carrito).ThenInclude(c => c.ItemsCompra).AsQueryable();

            if (orden == 1) compras = compras.OrderByDescending(c => c.Fecha); // Mas nuevas

            if (orden == 2) compras = compras.OrderBy(c => c.Fecha); // Mas antiguas

            return View(await compras.ToListAsync());
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var compra = _context.Compras
                .Include(c => c.Carrito)
                .ThenInclude(c => c.ItemsCompra)
                .ThenInclude(ic => ic.Producto)
                .ThenInclude(p => p.Categoria)
                .FirstOrDefault(c => c.CompraId == id);

            return View(compra);
        }

        // GET: Compras/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellido");
            return View();
        }

        // POST: Compras/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int carritoId, [Bind("CompraId,Fecha,PrecioFinal")] Compra compra)
        {
            // Devuelve el carrito.
            var carrito = _context.Carritos
               .Include(c => c.ItemsCompra).ThenInclude(ic => ic.Producto).ThenInclude(p => p.Categoria)
               .FirstOrDefault(cu => cu.CarritoId == carritoId);

            if (carrito == null) return NotFound();

            if (ModelState.IsValid)
            {
                compra.Estado = EstadoCompra.Finalizada;
                compra.UsuarioId = carrito.UsuarioId;
                compra.CarritoId = carrito.CarritoId;
                compra.Fecha = DateTime.Now;
                compra.PrecioFinal = CalcularPrecioFinal(carrito.ItemsCompra);
                carrito.Activo = false;

                foreach (ItemCompra item in carrito.ItemsCompra)
                {
                    Helpers.RemoverDeStock(item.Producto, item.Cantidad, _context);
                }

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                // Trabajamos con el carrito de la compra y el que tiene el usuario.
                Carrito nuevoCarrito = new Carrito()
                {
                    UsuarioId = carrito.UsuarioId,
                    Activo = true,
                };

                _context.Carritos.Add(nuevoCarrito);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellido", compra.UsuarioId);
            return View(compra);
        }

        // GET: Compras/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound();
            }

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellido", compra.UsuarioId);
            return View(compra);
        }

        // POST: Compras/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompraId,UsuarioId,Fecha,PrecioFinal")] Compra compra)
        {
            if (id != compra.CompraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompraExists(compra.CompraId))
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

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellido", compra.UsuarioId);
            return View(compra);
        }

        // GET: Compras/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CompraId == id);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // POST: Compras/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var compra = await _context.Compras.FindAsync(id);
            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.CompraId == id);
        }

        private double CalcularPrecioFinal(List<ItemCompra> items)
        {
            double total = 0;

            foreach (ItemCompra item in items)
            {
                total += item.Subtotal;
            }

            return total + (total * Helpers.IVA);
        }
    }
}
