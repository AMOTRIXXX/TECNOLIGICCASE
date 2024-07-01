using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TECNOLIGICCASE.Models
{
    public partial class Cargo
    {
        public int IdCargo { get; set; }

        [Required(ErrorMessage = "La descripción del cargo es requerida.")]
        [Remote(action: "VerifyDescripcion", controller: "Cargo", ErrorMessage = "Esta descripción del cargo ya está registrada.")]
        [RegularExpression(@"^\S(.*\S)?$", ErrorMessage = "El nombre del cargo no puede contener espacios en blanco al principio o al final.")]
        public string Descripcion { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}
