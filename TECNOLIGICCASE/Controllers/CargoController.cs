using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TECNOLIGICCASE.Models;

namespace TECNOLIGICCASE.Controllers
{
    public class CargoController : Controller
    {
        private readonly WebtecnologiaContext _context;

        public CargoController(WebtecnologiaContext context)
        {
            _context = context;
        }

        // GET: Cargo
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cargos.ToListAsync());
        }

        // GET: Cargo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos
                .FirstOrDefaultAsync(m => m.IdCargo == id);
            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // GET: Cargo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cargo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCargo,Descripcion,FechaCreacion,FechaModificacion")] Cargo cargo)
        {
            if (ModelState.IsValid)
            {
                // Verificar si la descripción del cargo ya existe en la base de datos
                if (_context.Cargos.Any(c => c.Descripcion == cargo.Descripcion))
                {
                    ModelState.AddModelError("Descripcion", "La descripción del cargo ya está en uso.");
                    return View(cargo);
                }

                _context.Add(cargo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cargo);
        }

        // GET: Cargo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos.FindAsync(id);
            if (cargo == null)
            {
                return NotFound();
            }
            return View(cargo);
        }

        // POST: Cargo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCargo,Descripcion,FechaCreacion,FechaModificacion")] Cargo cargo)
        {
            if (id != cargo.IdCargo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var cargoExistente = await _context.Cargos.FindAsync(id);
                    if (cargoExistente == null)
                    {
                        return NotFound();
                    }

                    // Crear registro en el historial de ediciones de cargo
                    _context.HistorialEdicionesCargo.Add(new HistorialEdicionesCargo
                    {
                        IdCargo = cargo.IdCargo,
                        Descripcion = cargoExistente.Descripcion,
                        DatosAntiguos = $"Descripción Cargo: {cargoExistente.Descripcion}",
                        DatosNuevos = $"Descripción Cargo: {cargo.Descripcion}",
                    });

                    // Actualizar el cargo existente
                    cargoExistente.Descripcion = cargo.Descripcion;
                    cargoExistente.FechaModificacion = DateTime.Now;

                    // Marca FechaCreacion como no modificada para evitar cambios
                    _context.Entry(cargoExistente).Property(e => e.FechaCreacion).IsModified = false;

                    // Guarda los cambios en el contexto
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CargoExists(cargo.IdCargo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
                    {
                        ModelState.AddModelError("Descripcion", "Esta descripción del cargo ya está registrada.");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cargo);
        }

        // GET: Cargo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos
                .FirstOrDefaultAsync(m => m.IdCargo == id);
            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // POST: Cargo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cargo = await _context.Cargos.FindAsync(id);
            if (cargo != null)
            {
                // Eliminar registros relacionados en HistorialEdicionesCargo
                var historial = await _context.HistorialEdicionesCargo
                    .Where(h => h.IdCargo == id)
                    .ToListAsync();

                _context.HistorialEdicionesCargo.RemoveRange(historial);

                _context.Cargos.Remove(cargo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CargoExists(int id)
        {
            return _context.Cargos.Any(e => e.IdCargo == id);
        }

        // Acción para validar la Descripcion
        [AcceptVerbs("Get", "Post")]
        public IActionResult VerifyDescripcion(string Descripcion)
        {
            if (_context.Cargos.Any(c => c.Descripcion == Descripcion))
            {
                return Json($"La descripción del cargo {Descripcion} ya está en uso.");
            }
            return Json(true);
        }
    }
}
