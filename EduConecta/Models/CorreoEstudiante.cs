using System.ComponentModel.DataAnnotations;

namespace EduConecta.Models;

public class CorreoEstudiante
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo no es valido.")]
    [StringLength(254)]
    [Display(Name = "Correo")]
    public string Direccion { get; set; } = string.Empty;

    public int EstudianteId { get; set; }

    public Estudiante Estudiante { get; set; } = null!;
}
