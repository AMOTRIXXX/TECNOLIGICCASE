using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TECNOLIGICCASE.Models
{
    public class HistorialEdicionesDepartamento
    {
        [Key]
        public int IdHistorial { get; set; }

        public int IdDepartamento { get; set; }

        [ForeignKey("IdDepartamento")]
        public Departamento Departamento { get; set; }

        public string NombreDepartamento { get; set; }

        public string DatosAntiguos { get; set; }

        public string DatosNuevos { get; set; }

        public DateTime FechaEdicion { get; set; } = DateTime.Now;
    }
}
