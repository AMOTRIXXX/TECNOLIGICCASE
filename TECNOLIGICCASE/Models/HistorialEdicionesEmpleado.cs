using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TECNOLIGICCASE.Models
{
    public class HistorialEdicionesEmpleado
    {
        public int IdHistorial { get; set; }
        public int IdEmpleado { get; set; }
        public string NoDocumento { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string EntidadesEditadas { get; set; }
        public string DatosAntiguos { get; set; }
        public string DatosNuevos { get; set; }
        public DateTime FechaEdicion { get; set; }
        public virtual Empleado Empleado { get; set; }

        [NotMapped]
        public string NombreDepartamentoAntiguo { get; set; }
        [NotMapped]
        public string DescripcionCargoAntiguo { get; set; }
        [NotMapped]
        public string NombreDepartamentoNuevo { get; set; }
        [NotMapped]
        public string DescripcionCargoNuevo { get; set; }
    }
}
