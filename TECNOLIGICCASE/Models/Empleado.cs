using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
public class NoWhiteSpaceAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string str && str.Contains(" "))
        {
            return new ValidationResult("El campo no debe contener espacios en blanco.");
        }

        return ValidationResult.Success;
    }
}
namespace TECNOLIGICCASE.Models
{
    public partial class Empleado
    {
        public int IdEmpleado { get; set; }

        public string? TipoDocumento { get; set; }

        [Required(ErrorMessage = "El NoDocumento es requerido.")]
        [NoWhiteSpace(ErrorMessage = "El NoDocumento no debe contener espacios en blanco.")]
        public string NoDocumento { get; set; }

        public string? Nombres { get; set; }

        public string? Apellidos { get; set; }

        public int? IdDepart { get; set; }

        public int? IdCargo { get; set; }


        public string? Ciudad { get; set; }

        public string? Direccion { get; set; }


        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
        [NoWhiteSpace(ErrorMessage = "El correo electrónico no debe contener espacios en blanco.")]
        public string? Email { get; set; }

        [NoWhiteSpace(ErrorMessage = "El Telefono no debe contener espacios en blanco.")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El campo Sueldo es requerido.")]
        public decimal? Sueldo { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        // Método para actualizar la fecha de modificación
        public void ActualizarFechaModificacion()
        {
            FechaModificacion = DateTime.Now;
        }

        public virtual Cargo? oCargo { get; set; }

        public virtual Departamento? oDepart { get; set; }

        // Nueva colección para el historial de ediciones
        public virtual ICollection<HistorialEdicionesEmpleado> HistorialEdiciones { get; set; } = new List<HistorialEdicionesEmpleado>();


        // Propiedades de solo lectura para Descripcion y NombreDepartamento
        public string Descripcion
        {
            get { return oCargo != null ? oCargo.Descripcion : string.Empty; }
        }

        public string NombreDepartamento
        {
            get { return oDepart != null ? oDepart.NombreDepartamento : string.Empty; }
        }
        // Nueva propiedad calculada para la antigüedad
        public string Antiguedad
        {
            get
            {
                if (FechaCreacion == null)
                {
                    return "Fecha de creación no disponible";
                }

                TimeSpan diferencia = DateTime.Now - FechaCreacion.Value;
                int años = (int)(diferencia.Days / 365.25);
                int meses = (int)((diferencia.Days % 365.25) / 30.44);
                int días = (int)((diferencia.Days % 365.25) % 30.44);
                int horas = diferencia.Hours;

                return $"{años} años, {meses} meses, {días} días, {horas} horas";
            }
        }
        // Método para crear un registro de historial de ediciones
        public async Task<HistorialEdicionesEmpleado> CrearHistorialEdicionAsync(WebtecnologiaContext context, string entidadesEditadas, Empleado datosAntiguos, Empleado datosNuevos)
        {
            // Obtener nombres de departamentos y descripciones de cargos
            datosAntiguos.oDepart = await context.Departamentos.FindAsync(datosAntiguos.IdDepart);
            datosAntiguos.oCargo = await context.Cargos.FindAsync(datosAntiguos.IdCargo);
            datosNuevos.oDepart = await context.Departamentos.FindAsync(datosNuevos.IdDepart);
            datosNuevos.oCargo = await context.Cargos.FindAsync(datosNuevos.IdCargo);

            // Actualizar fecha de modificación
            datosAntiguos.ActualizarFechaModificacion();
            datosNuevos.ActualizarFechaModificacion();

            // Serializar datos antiguos y nuevos con nombres de departamentos y descripciones de cargos
            var datosAntiguosJson = JsonConvert.SerializeObject(datosAntiguos, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            var datosNuevosJson = JsonConvert.SerializeObject(datosNuevos, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return new HistorialEdicionesEmpleado
            {
                IdEmpleado = this.IdEmpleado,
                NoDocumento = this.NoDocumento,
                Nombres = this.Nombres,
                Apellidos = this.Apellidos,
                EntidadesEditadas = entidadesEditadas,
                DatosAntiguos = datosAntiguosJson,
                DatosNuevos = datosNuevosJson,
                FechaEdicion = DateTime.Now
            };
        }
    }
}