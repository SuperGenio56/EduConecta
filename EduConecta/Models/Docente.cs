using System.ComponentModel.DataAnnotations;

namespace EduConecta.Models;

public class Docente
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(120)]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    [StringLength(80)]
    public string Especialidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo no es valido.")]
    [StringLength(254)]
    public string Correo { get; set; } = string.Empty;

    public ICollection<CursoDocente> CursoDocentes { get; set; } = new List<CursoDocente>();
}
