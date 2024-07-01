using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TECNOLIGICCASE.Models;

namespace TECNOLIGICCASE.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly WebtecnologiaContext _context;

        // Constructor del controlador
        public EmpleadoController(WebtecnologiaContext context)
        {
            _context = context;
        }

        // GET: Empleado - Método para listar todos los empleados
        public async Task<IActionResult> Index()
        {
            // Incluye las relaciones con Cargo y Departamento al obtener la lista de empleados
            var webtecnologiaContext = _context.Empleados.Include(e => e.oCargo).Include(e => e.oDepart);
            // Espera a que la operación de obtener la lista de empleados de la base de datos se complete
            return View(await webtecnologiaContext.ToListAsync());
        }

        // GET: Empleado/Details/5 - Método para mostrar detalles de un empleado
        public async Task<IActionResult> Details(int? id)
        {
            // Verifica si el ID proporcionado es nulo
            if (id == null)
            {
                return NotFound();
            }
            // Obtiene los detalles de un empleado incluyendo su cargo y departamento de manera asincrónica
            var empleado = await _context.Empleados
                .Include(e => e.oCargo)
                .Include(e => e.oDepart)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);
            // Verifica si el empleado no existe
            if (empleado == null)
            {
                return NotFound();
            }

            // Muestra la vista con los detalles del empleado
            return View(empleado);
        }

        // GET: Empleado/Create - Método para mostrar el formulario de creación de un nuevo empleado
        public IActionResult Create()
        {
            // Prepara las listas desplegables para el formulario de creación
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "Descripcion");
            ViewData["IdDepart"] = new SelectList(_context.Departamentos, "IdDepartamento", "NombreDepartamento");
            ViewData["TipoDocumento"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Cédula de Ciudadanía", Value = "CC" },
                new SelectListItem { Text = "Tarjeta de Identidad", Value = "TI" },
                new SelectListItem { Text = "Pasaporte", Value = "PA" },
                new SelectListItem { Text = "Cédula de Extranjería", Value = "CE" },
                new SelectListItem { Text = "Registro Civil", Value = "RC" }
            }, "Value", "Text");
            // Muestra la vista del formulario de creación
            return View();
        }

        // POST: Empleado/Create - Método para manejar el envío del formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleado,TipoDocumento,NoDocumento,Nombres,Apellidos,IdDepart,IdCargo,Ciudad,Direccion,Email,Telefono,Sueldo,FechaCreacion,FechaModificacion")] Empleado empleado)
        {
            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                // Verificar si hay campos vacíos o contienen solo espacios
                if (string.IsNullOrWhiteSpace(empleado.TipoDocumento) ||
                    string.IsNullOrWhiteSpace(empleado.NoDocumento) ||
                    string.IsNullOrWhiteSpace(empleado.Nombres) ||
                    string.IsNullOrWhiteSpace(empleado.Apellidos) ||
                    string.IsNullOrWhiteSpace(empleado.Email) ||
                    string.IsNullOrWhiteSpace(empleado.Telefono) ||
                    empleado.Sueldo == null) // Verificar si el Sueldo es nulo
                {
                    // Agrega un error al estado del modelo
                    ModelState.AddModelError(string.Empty, "Todos los campos son requeridos.");
                    // Prepara las listas desplegables para el formulario de creación
                    ViewBag.IdCargo = new SelectList(_context.Cargos, "IdCargo", "Descripcion", empleado.IdCargo);
                    ViewBag.IdDepart = new SelectList(_context.Departamentos, "IdDepartamento", "NombreDepartamento", empleado.IdDepart);
                    ViewData["TipoDocumento"] = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Cédula de Ciudadanía", Value = "CC" },
                        new SelectListItem { Text = "Tarjeta de Identidad", Value = "TI" },
                        new SelectListItem { Text = "Pasaporte", Value = "PA" },
                        new SelectListItem { Text = "Cédula de Extranjería", Value = "CE" },
                        new SelectListItem { Text = "Registro Civil", Value = "RC" }
                    }, "Value", "Text", empleado.TipoDocumento);
                    // Retorna la vista con el empleado para corregir los errores
                    return View(empleado);
                }

                // Verificar si el correo electrónico ya existe en la base de datos
                if (_context.Empleados.Any(e => e.Email == empleado.Email))
                {
                    // Agrega un error al estado del modelo
                    ModelState.AddModelError("Email", "El correo electrónico ya está en uso.");
                    // Prepara las listas desplegables para el formulario de creación
                    ViewBag.IdCargo = new SelectList(_context.Cargos, "IdCargo", "Descripcion", empleado.IdCargo);
                    ViewBag.IdDepart = new SelectList(_context.Departamentos, "IdDepartamento", "NombreDepartamento", empleado.IdDepart);
                    ViewData["TipoDocumento"] = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Cédula de Ciudadanía", Value = "CC" },
                        new SelectListItem { Text = "Tarjeta de Identidad", Value = "TI" },
                        new SelectListItem { Text = "Pasaporte", Value = "PA" },
                        new SelectListItem { Text = "Cédula de Extranjería", Value = "CE" },
                        new SelectListItem { Text = "Registro Civil", Value = "RC" }
                    }, "Value", "Text", empleado.TipoDocumento);
                    // Retorna la vista con el empleado para corregir los errores
                    return View(empleado);
                }

                // Verificar si el NoDocumento ya está en uso
                if (_context.Empleados.Any(e => e.NoDocumento == empleado.NoDocumento))
                {
                    // Agrega un error al estado del modelo
                    ModelState.AddModelError("NoDocumento", "Este NoDocumento ya está registrado. Por favor, utiliza otro número.");
                    // Prepara las listas desplegables para el formulario de creación
                    ViewBag.IdCargo = new SelectList(_context.Cargos, "IdCargo", "Descripcion", empleado.IdCargo);
                    ViewBag.IdDepart = new SelectList(_context.Departamentos, "IdDepartamento", "NombreDepartamento", empleado.IdDepart);
                    ViewData["TipoDocumento"] = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Cédula de Ciudadanía", Value = "CC" },
                        new SelectListItem { Text = "Tarjeta de Identidad", Value = "TI" },
                        new SelectListItem { Text = "Pasaporte", Value = "PA" },
                        new SelectListItem { Text = "Cédula de Extranjería", Value = "CE" },
                        new SelectListItem { Text = "Registro Civil", Value = "RC" }
                    }, "Value", "Text", empleado.TipoDocumento);
                    // Retorna la vista con el empleado para corregir los errores
                    return View(empleado);
                }

                // Asigna la fecha de creación y modificación actuales
                empleado.FechaCreacion = DateTime.Now;
                empleado.FechaModificacion = DateTime.Now;
                // Añade el nuevo empleado al contexto
                _context.Add(empleado);
                // Guarda los cambios de manera asincrónica
                await _context.SaveChangesAsync();
                // Redirige a la acción Index
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, prepara las listas desplegables y retorna la vista con el empleado
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "Descripcion", empleado.IdCargo);
            ViewData["IdDepart"] = new SelectList(_context.Departamentos, "IdDepartamento", "NombreDepartamento", empleado.IdDepart);
            ViewData["TipoDocumento"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Cédula de Ciudadanía", Value = "CC" },
                new SelectListItem { Text = "Tarjeta de Identidad", Value = "TI" },
                new SelectListItem { Text = "Pasaporte", Value = "PA" },
                new SelectListItem { Text = "Cédula de Extranjería", Value = "CE" },
                new SelectListItem { Text = "Registro Civil", Value = "RC" }
            }, "Value", "Text", empleado.TipoDocumento);

            // Añadir validación del campo Sueldo
            if (empleado.Sueldo == null)
            {
                ModelState.AddModelError("Sueldo", "El campo Sueldo es requerido.");
            }
            else if (!decimal.TryParse(empleado.Sueldo.ToString(), out decimal sueldo))
            {
                ModelState.AddModelError("Sueldo", "El campo Sueldo debe ser un número.");
            }

            // Retorna la vista con el empleado para corregir los errores
            return View(empleado);
        }

        // GET: Empleado/Edit/5 - Método para mostrar el formulario de edición de un empleado
        public async Task<IActionResult> Edit(int? id)
        {
            // Verifica si el ID proporcionado es nulo
            if (id == null)
            {
                return NotFound();
            }

            // Busca el empleado en la base de datos de manera asincrónica
            var empleado = await _context.Empleados.FindAsync(id);
            // Verifica si el empleado no existe
            if (empleado == null)
            {
                return NotFound();
            }

            // Prepara las listas desplegables para el formulario de edición
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "Descripcion", empleado.IdCargo);
            ViewData["IdDepart"] = new SelectList(_context.Departamentos, "IdDepartamento", "NombreDepartamento", empleado.IdDepart);
            ViewData["TipoDocumento"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Cédula de Ciudadanía", Value = "CC" },
                new SelectListItem { Text = "Tarjeta de Identidad", Value = "TI" },
                new SelectListItem { Text = "Pasaporte", Value = "PA" },
                new SelectListItem { Text = "Cédula de Extranjería", Value = "CE" },
                new SelectListItem { Text = "Registro Civil", Value = "RC" }
            }, "Value", "Text", empleado.TipoDocumento);
            // Muestra la vista del formulario de edición
            return View(empleado);
        }

        // POST: Empleado/Edit/5 - Método para manejar el envío del formulario de edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleado,TipoDocumento,NoDocumento,Nombres,Apellidos,IdDepart,IdCargo,Ciudad,Direccion,Email,Telefono,Sueldo,FechaCreacion,FechaModificacion")] Empleado empleado)
        {
            // Verifica si el ID proporcionado no coincide con el ID del empleado
            if (id != empleado.IdEmpleado)
            {
                return NotFound();
            }

            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                // Verificar si hay campos vacíos o contienen solo espacios
                if (string.IsNullOrWhiteSpace(empleado.TipoDocumento) ||
                    string.IsNullOrWhiteSpace(empleado.NoDocumento) ||
                    string.IsNullOrWhiteSpace(empleado.Nombres) ||
                    string.IsNullOrWhiteSpace(empleado.Apellidos) ||
                    string.IsNullOrWhiteSpace(empleado.Email) ||
                    string.IsNullOrWhiteSpace(empleado.Telefono) ||
                    empleado.Sueldo == null) // Verificar si el Sueldo es nulo
                {
                    // Agrega un error al estado del modelo
                    ModelState.AddModelError(string.Empty, "Todos los campos son requeridos y no pueden contener solo espacios.");
                    // Prepara las listas desplegables para el formulario de edición
                    ViewBag.IdCargo = new SelectList(_context.Cargos, "IdCargo", "Descripcion", empleado.IdCargo);
                    ViewBag.IdDepart = new SelectList(_context.Departamentos, "IdDepartamento", "NombreDepartamento", empleado.IdDepart);
                    ViewData["TipoDocumento"] = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Cédula de Ciudadanía", Value = "CC" },
                        new SelectListItem { Text = "Tarjeta de Identidad", Value = "TI" },
                        new SelectListItem { Text = "Pasaporte", Value = "PA" },
                        new SelectListItem { Text = "Cédula de Extranjería", Value = "CE" },
                        new SelectListItem { Text = "Registro Civil", Value = "RC" }
                    }, "Value", "Text", empleado.TipoDocumento);
                    // Retorna la vista con el empleado para corregir los errores
                    return View(empleado);
                }
                // Verificar si el NoDocumento ya está en uso por otro empleado
                var existingEmployee = await _context.Empleados
                    .FirstOrDefaultAsync(e => e.NoDocumento == empleado.NoDocumento && e.IdEmpleado != empleado.IdEmpleado);
                if (existingEmployee != null)
                {
                    // Agrega un error al estado del modelo
                    ModelState.AddModelError("NoDocumento", "Este número de documento ya está registrado. Por favor, utiliza otro número.");
                    // Prepara las listas desplegables para el formulario de edición
                    ViewBag.IdCargo = new SelectList(_context.Cargos, "IdCargo", "Descripcion", empleado.IdCargo);
                    ViewBag.IdDepart = new SelectList(_context.Departamentos, "IdDepartamento", "NombreDepartamento", empleado.IdDepart);
                    ViewData["TipoDocumento"] = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Cédula de Ciudadanía", Value = "CC" },
                        new SelectListItem { Text = "Tarjeta de Identidad", Value = "TI" },
                        new SelectListItem { Text = "Pasaporte", Value = "PA" },
                        new SelectListItem { Text = "Cédula de Extranjería", Value = "CE" },
                        new SelectListItem { Text = "Registro Civil", Value = "RC" }
                    }, "Value", "Text", empleado.TipoDocumento);
                    // Retorna la vista con el empleado para corregir los errores
                    return View(empleado);
                }

                try
                {

                    var empleadoAntiguo = await _context.Empleados.AsNoTracking()
                .Include(e => e.oDepart)
                .Include(e => e.oCargo)
                .FirstOrDefaultAsync(e => e.IdEmpleado == empleado.IdEmpleado);

                    var historialEdicion = await empleadoAntiguo.CrearHistorialEdicionAsync(_context, "Empleado", empleadoAntiguo, empleado);

                    _context.HistorialEdicionesEmpleados.Add(historialEdicion);
                    // Actualiza los valores del empleado existente con los nuevos valores del formulario
                    _context.Entry(empleado).CurrentValues.SetValues(empleado);

                    // Actualiza la fecha de modificación con la fecha y hora actuales
                    empleado.FechaModificacion = DateTime.Now;

                    

                    // Actualiza el empleado en el contexto
                    _context.Update(empleado);
                    // Marca FechaCreacion como no modificada para evitar cambios
                    _context.Entry(empleado).Property(e => e.FechaCreacion).IsModified = false;
                    // Guarda los cambios de manera asincrónica
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Verifica si el empleado no existe
                    if (!EmpleadoExists(empleado.IdEmpleado))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Redirige a la acción Index
                return RedirectToAction(nameof(Index));
            }
            // Prepara las listas desplegables para el formulario de edición y retorna la vista con el empleado
            ViewBag.IdCargo = new SelectList(_context.Cargos, "IdCargo", "Descripcion", empleado.IdCargo);
            ViewBag.IdDepart = new SelectList(_context.Departamentos, "IdDepartamento", "NombreDepartamento", empleado.IdDepart);
            ViewData["TipoDocumento"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Cédula de Ciudadanía", Value = "CC" },
                new SelectListItem { Text = "Tarjeta de Identidad", Value = "TI" },
                new SelectListItem { Text = "Pasaporte", Value = "PA" },
                new SelectListItem { Text = "Cédula de Extranjería", Value = "CE" },
                new SelectListItem { Text = "Registro Civil", Value = "RC" }
            }, "Value", "Text", empleado.TipoDocumento);
            // Retorna la vista con el empleado para corregir los errores
            return View(empleado);
        }

        // GET: Empleado/Delete/5 - Método para mostrar la confirmación de eliminación de un empleado
        public async Task<IActionResult> Delete(int? id)
        {
            // Verifica si el ID proporcionado es nulo
            if (id == null)
            {
                return NotFound();
            }

            // Obtiene el empleado a eliminar incluyendo su cargo y departamento de manera asincrónica
            var empleado = await _context.Empleados
                .Include(e => e.oCargo)
                .Include(e => e.oDepart)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);
            // Verifica si el empleado no existe
            if (empleado == null)
            {
                return NotFound();
            }

            // Muestra la vista con los detalles del empleado a eliminar
            return View(empleado);
        }

        // POST: Empleado/Delete/5 - Método para manejar la confirmación de eliminación de un empleado
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Busca el empleado en la base de datos de manera asincrónica
            var empleado = await _context.Empleados.FindAsync(id);
            // Si el empleado existe, lo elimina del contexto
            if (empleado != null)
            {
                // Eliminar registros relacionados en HistorialEdicionesEmpleado
                var historial = await _context.HistorialEdicionesEmpleados
                    .Where(h => h.IdEmpleado == id)
                    .ToListAsync();

                _context.HistorialEdicionesEmpleados.RemoveRange(historial);
                _context.Empleados.Remove(empleado);
            }

            // Guarda los cambios de manera asincrónica
            await _context.SaveChangesAsync();
            // Redirige a la acción Index
            return RedirectToAction(nameof(Index));
        }

        // Método para verificar si un empleado existe
        private bool EmpleadoExists(int id)
        {
            // Verifica si existe un empleado con el ID proporcionado
            return _context.Empleados.Any(e => e.IdEmpleado == id);
        }

        // Acción para validar el NoDocumento
        [AcceptVerbs("Get", "Post")]
        public IActionResult VerifyNoDocumento(string NoDocumento, int IdEmpleado)
        {
            // Busca el empleado en la base de datos
            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == IdEmpleado);
            // Si el número de documento pertenece al mismo empleado, retorna éxito
            if (empleado != null && empleado.NoDocumento == NoDocumento)
            {
                return Json(true);
            }
            // Retorna éxito por defecto (puede modificar según la lógica de validación necesaria)
            return Json(true);
        }
        // Resto de acciones del controlador Empleado

        // GET: Empleado/Historial/5 - Método para mostrar el historial de ediciones de un empleado
        public async Task<IActionResult> Historial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historialEdiciones = await _context.HistorialEdicionesEmpleados
                .Where(h => h.IdEmpleado == id)
                .OrderByDescending(h => h.FechaEdicion)
                .ToListAsync();

            if (historialEdiciones == null || historialEdiciones.Count == 0)
            {
                return NotFound("No hay registros de historial para este empleado.");
            }

            return View(historialEdiciones);
        }
    }
}
