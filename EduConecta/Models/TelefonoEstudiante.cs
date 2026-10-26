using System.ComponentModel.DataAnnotations;

namespace EduConecta.Models;

public class TelefonoEstudiante
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El telefono es obligatorio.")]
    [StringLength(20)]
    [RegularExpression(@"^[0-9+\-\s]{7,20}$",
        ErrorMessage = "El telefono solo admite numeros, espacios, el signo mas y guiones.")]
    [Display(Name = "Telefono")]
    public string Numero { get; set; } = string.Empty;

    public int EstudianteId { get; set; }

    public Estudiante Estudiante { get; set; } = null!;
}
