using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TECNOLIGICCASE.Models
{
    public class HistorialEdicionesCargo
    {
        [Key]
        public int IdHistorial { get; set; }

        public int IdCargo { get; set; }

        [ForeignKey("IdCargo")]
        public Cargo Cargo { get; set; }

        public string Descripcion { get; set; }

        public string DatosAntiguos { get; set; }

        public string DatosNuevos { get; set; }

        public DateTime FechaEdicion { get; set; } = DateTime.Now;
    }
}
