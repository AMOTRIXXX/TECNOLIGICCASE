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
    public class DepartamentoController : Controller
    {
        private readonly WebtecnologiaContext _context;

        public DepartamentoController(WebtecnologiaContext context)
        {
            _context = context;
        }

        // GET: Departamento
        public async Task<IActionResult> Index()
        {
            return View(await _context.Departamentos.ToListAsync());
        }

        // GET: Departamento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamentos
                .FirstOrDefaultAsync(m => m.IdDepartamento == id);
            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        // GET: Departamento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDepartamento,NombreDepartamento,FechaCreacion,FechaModificacion")] Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el nombre del departamento ya existe en la base de datos
                if (_context.Departamentos.Any(d => d.NombreDepartamento == departamento.NombreDepartamento))
                {
                    ModelState.AddModelError("NombreDepartamento", "El nombre del departamento ya está en uso.");
                    return View(departamento);
                }

                _context.Add(departamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(departamento);
        }

        // GET: Departamento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamentos.FindAsync(id);
            if (departamento == null)
            {
                return NotFound();
            }
            return View(departamento);
        }

        // POST: Departamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDepartamento,NombreDepartamento,FechaCreacion,FechaModificacion")] Departamento departamento)
        {
            if (id != departamento.IdDepartamento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var departamentoExistente = await _context.Departamentos.FindAsync(id);
                    if (departamentoExistente == null)
                    {
                        return NotFound();
                    }

                    // Crear registro en el historial de ediciones de departamento
                    _context.HistorialEdicionesDepartamento.Add(new HistorialEdicionesDepartamento
                    {
                        IdDepartamento = departamento.IdDepartamento,
                        NombreDepartamento = departamentoExistente.NombreDepartamento,
                        DatosAntiguos = $"Nombre Departamento: {departamentoExistente.NombreDepartamento}",
                        DatosNuevos = $"Nombre Departamento: {departamento.NombreDepartamento}",
                    });

                    // Actualizar las propiedades del departamento existente con los nuevos valores del formulario
                    departamentoExistente.NombreDepartamento = departamento.NombreDepartamento;
                    departamentoExistente.FechaModificacion = DateTime.Now;

                    // Marca FechaCreacion como no modificada para evitar cambios
                    _context.Entry(departamentoExistente).Property(e => e.FechaCreacion).IsModified = false;

                    // Guardar cambios en el contexto
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartamentoExists(departamento.IdDepartamento))
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
                        ModelState.AddModelError("NombreDepartamento", "Este nombre de departamento ya está registrado.");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(departamento);
        }

        // GET: Departamento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamentos
                .FirstOrDefaultAsync(m => m.IdDepartamento == id);
            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        // POST: Departamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var departamento = await _context.Departamentos.FindAsync(id);
            if (departamento != null)
            {
                // Eliminar registros relacionados en HistorialEdicionesDepartamento
                var historial = await _context.HistorialEdicionesDepartamento
                    .Where(h => h.IdDepartamento == id)
                    .ToListAsync();

                _context.HistorialEdicionesDepartamento.RemoveRange(historial);
                _context.Departamentos.Remove(departamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartamentoExists(int id)
        {
            return _context.Departamentos.Any(e => e.IdDepartamento == id);
        }

        // Acción para validar el NombreDepartamento
        [AcceptVerbs("Get", "Post")]
        public IActionResult VerifyNombreDepartamento(string NombreDepartamento)
        {
            if (_context.Departamentos.Any(d => d.NombreDepartamento == NombreDepartamento))
            {
                return Json($"El nombre del departamento {NombreDepartamento} ya está en uso.");
            }
            return Json(true);
        }
    }
}
