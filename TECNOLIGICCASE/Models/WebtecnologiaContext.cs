using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TECNOLIGICCASE.Models
{
    public partial class WebtecnologiaContext : DbContext
    {
        public WebtecnologiaContext()
        {
        }

        public WebtecnologiaContext(DbContextOptions<WebtecnologiaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cargo> Cargos { get; set; }

        public virtual DbSet<Departamento> Departamentos { get; set; }

        public virtual DbSet<Empleado> Empleados { get; set; }
        public DbSet<HistorialEdicionesEmpleado> HistorialEdicionesEmpleados { get; set; }
        public DbSet<HistorialEdicionesDepartamento> HistorialEdicionesDepartamento { get; set; }

        public DbSet<HistorialEdicionesCargo> HistorialEdicionesCargo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cargo>(entity =>
            {
                entity.HasKey(e => e.IdCargo).HasName("PK__CARGO__6C9856254547DA30");

                entity.ToTable("CARGO", tb =>
                {
                    tb.HasTrigger("trg_InsertFechaCreacionCargo");
                    tb.HasTrigger("trg_UpdateFechaModificacionCargo");
                });

                entity.HasIndex(e => e.Descripcion, "UQ__CARGO__92C53B6CADB39446").IsUnique();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAdd(); // Indica que este campo se genera al añadir
                entity.Property(e => e.FechaModificacion)
                   .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                       .ValueGeneratedOnAdd(); // Indica que este campo se genera  actualizar
            });

            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.HasKey(e => e.IdDepartamento).HasName("PK__DEPARTAM__787A433D9E466FBB");

                entity.ToTable("DEPARTAMENTO", tb =>
                {
                    tb.HasTrigger("trg_InsertFechaCreacionDepartamento");
                    tb.HasTrigger("trg_UpdateFechaModificacionDepartamento");
                });

                entity.HasIndex(e => e.NombreDepartamento, "UQ__DEPARTAM__2B0383DC5009095E").IsUnique();

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAdd(); // Indica que este campo se genera al añadir
                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                       .ValueGeneratedOnAdd(); // Indica que este campo se genera  actualizar
                entity.Property(e => e.NombreDepartamento)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.HasKey(e => e.IdEmpleado).HasName("PK__EMPLEADO__CE6D8B9E18D949E1");

                entity.ToTable("EMPLEADO", tb =>
                {
                    tb.HasTrigger("trg_InsertFechaCreacionEmpleado");
                    tb.HasTrigger("trg_UpdateFechaModificacionEmpleado");
                });

                entity.HasIndex(e => e.Email, "UQ__EMPLEADO__A9D10534DF21268F").IsUnique();

                entity.HasIndex(e => e.NoDocumento, "UQ__EMPLEADO__BFBAD14A87144814").IsUnique();

                entity.Property(e => e.Apellidos)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Ciudad)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Direccion)
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAdd(); // Indica que este campo se genera al añadir
                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                       .ValueGeneratedOnAdd(); // Indica que este campo se genera  actualizar
                entity.Property(e => e.NoDocumento)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.Nombres)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Sueldo).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Telefono)
                    .HasMaxLength(15)
                    .IsUnicode(false);
                entity.Property(e => e.TipoDocumento)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("tipo_documento");

                entity.HasOne(d => d.oCargo).WithMany(p => p.Empleados)
                    .HasForeignKey(d => d.IdCargo)
                    .HasConstraintName("FK_Cargo");

                entity.HasOne(d => d.oDepart).WithMany(p => p.Empleados)
                    .HasForeignKey(d => d.IdDepart)
                    .HasConstraintName("FK_Departamento");
            });

            modelBuilder.Entity<HistorialEdicionesEmpleado>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("PK__Historia__9CC7DBB45698434C");

            entity.ToTable("HistorialEdicionesEmpleado");

            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DatosAntiguos).IsUnicode(false);
            entity.Property(e => e.DatosNuevos).IsUnicode(false);
            entity.Property(e => e.EntidadesEditadas).IsUnicode(false);
            entity.Property(e => e.FechaEdicion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
            .ValueGeneratedOnAdd();
            entity.Property(e => e.NoDocumento)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Empleado).WithMany(p => p.HistorialEdiciones)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("FK_HistorialEdicionesEmpleado_Empleado");
        });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}