using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduConecta.Models;

public class Matricula
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El periodo academico es obligatorio.")]
    [StringLength(10)]
    [RegularExpression(@"^\d{4}-[12]$",
        ErrorMessage = "El periodo debe tener el formato 2026-1 o 2026-2.")]
    [Display(Name = "Periodo academico")]
    public string PeriodoAcademico { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de matricula")]
    public DateTime FechaMatricula { get; set; }

    public EstadoMatricula Estado { get; set; }

    [Range(0, 100, ErrorMessage = "La nota final debe estar entre 0 y 100.")]
    [Display(Name = "Nota final")]
    public decimal? NotaFinal { get; set; }

    [Display(Name = "Estudiante")]
    public int EstudianteId { get; set; }

    [Display(Name = "Curso")]
    public int CursoId { get; set; }

    [Display(Name = "Beca")]
    public int? BecaId { get; set; }

    public Estudiante Estudiante { get; set; } = null!;
    public Curso Curso { get; set; } = null!;
    public Beca? Beca { get; set; }

    [NotMapped]
    [Display(Name = "Monto becado")]
    public decimal MontoBecado =>
        Curso == null || Beca == null
            ? 0m
            : Math.Round(Curso.CostoMatricula * Beca.PorcentajeDescuento / 100m, 2);

    [NotMapped]
    [Display(Name = "Monto a pagar")]
    public decimal MontoAPagar => Curso == null ? 0m : Curso.CostoMatricula - MontoBecado;
}
