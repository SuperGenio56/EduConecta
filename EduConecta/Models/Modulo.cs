using System.ComponentModel.DataAnnotations;

namespace EduConecta.Models;

public class Modulo
{
    [Display(Name = "Curso")]
    public int CursoId { get; set; }

    [Range(1, 50, ErrorMessage = "El numero de modulo debe estar entre 1 y 50.")]
    [Display(Name = "Numero de modulo")]
    public int NumeroModulo { get; set; }

    [Required(ErrorMessage = "El nombre del modulo es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Nombre del modulo")]
    public string NombreModulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es obligatorio.")]
    [StringLength(500)]
    public string Contenido { get; set; } = string.Empty;

    [Range(1, 200, ErrorMessage = "Las horas estimadas deben estar entre 1 y 200.")]
    [Display(Name = "Horas estimadas")]
    public int HorasEstimadas { get; set; }

    public Curso Curso { get; set; } = null!;
}
