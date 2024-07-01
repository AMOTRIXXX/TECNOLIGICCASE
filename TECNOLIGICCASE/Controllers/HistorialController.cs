using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TECNOLIGICCASE.Models;

namespace TECNOLIGICCASE.Controllers
{
    public class HistorialController : Controller
    {
        private readonly WebtecnologiaContext _context;

        public HistorialController(WebtecnologiaContext context)
        {
            _context = context;
        }

        // GET: Historial/Index
        public async Task<IActionResult> Index()
        {
            var historialEdiciones = await _context.HistorialEdicionesEmpleados.ToListAsync();
            return View(historialEdiciones);
        }
        // GET: Historial/IndexDepartamento
        public async Task<IActionResult> IndexDepartamento()
        {
            var historialDepartamentos = await _context.HistorialEdicionesDepartamento
                .Include(h => h.Departamento)
                .OrderByDescending(h => h.FechaEdicion)
                .ToListAsync();

            return View("IndexDepartamento", historialDepartamentos);
        }

        // GET: Historial/IndexCargo
        public async Task<IActionResult> IndexCargo()
        {
            var historialCargos = await _context.HistorialEdicionesCargo
                .Include(h => h.Cargo)
                .OrderByDescending(h => h.FechaEdicion)
                .ToListAsync();

            return View("IndexCargo", historialCargos);
        }
    }
}
