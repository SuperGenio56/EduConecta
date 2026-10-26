using System.ComponentModel.DataAnnotations;

namespace EduConecta.Models;

public class Sede
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El pais es obligatorio.")]
    [StringLength(40)]
    [Display(Name = "Pais")]
    public string Pais { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es obligatoria.")]
    [StringLength(60)]
    public string Ciudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del campus es obligatorio.")]
    [StringLength(80)]
    [Display(Name = "Nombre del campus")]
    public string NombreCampus { get; set; } = string.Empty;

    public ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
}
