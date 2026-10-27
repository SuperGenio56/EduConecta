using System.ComponentModel.DataAnnotations;

namespace EduConecta.Models;

public class Beca
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El tipo de beca es obligatorio.")]
    [StringLength(60)]
    [Display(Name = "Tipo de beca")]
    public string TipoBeca { get; set; } = string.Empty;

    [Range(1, 100, ErrorMessage = "El porcentaje debe estar entre 1 y 100.")]
    [Display(Name = "Porcentaje de descuento")]
    public decimal PorcentajeDescuento { get; set; }

    [Display(Name = "Estudiante")]
    public int EstudianteId { get; set; }

    public Estudiante Estudiante { get; set; } = null!;

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
