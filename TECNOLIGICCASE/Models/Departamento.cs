using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TECNOLIGICCASE.Models
{
    public partial class Departamento
    {
        public int IdDepartamento { get; set; }

        [Required(ErrorMessage = "El nombre del departamento es requerido.")]
        [Remote(action: "VerifyNombreDepartamento", controller: "Departamento", ErrorMessage = "Este nombre de departamento ya está registrado.")]
        [RegularExpression(@"^\S(.*\S)?$", ErrorMessage = "El nombre del departamento no puede contener espacios en blanco al principio o al final.")]
        public string NombreDepartamento { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

        // Validación personalizada para asegurar que no haya espacios en blanco
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(NombreDepartamento))
            {
                yield return new ValidationResult("El nombre del departamento no puede estar vacío o contener solo espacios en blanco.", new[] { nameof(NombreDepartamento) });
            }
            if (NombreDepartamento != null && NombreDepartamento.Contains(" "))
            {
                yield return new ValidationResult("El nombre del departamento no puede contener espacios en blanco.", new[] { nameof(NombreDepartamento) });
            }
        }
    }
}