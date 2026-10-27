using System.ComponentModel.DataAnnotations;

namespace EduConecta.Models;

public class Curso
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El codigo del curso es obligatorio.")]
    [StringLength(15)]
    [Display(Name = "Codigo")]
    public string CodigoCurso { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del curso es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Nombre del curso")]
    public string NombreCurso { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Los creditos deben estar entre 1 y 20.")]
    public int Creditos { get; set; }

    public ModalidadCurso Modalidad { get; set; }

    [Range(1, 500, ErrorMessage = "El cupo maximo debe estar entre 1 y 500.")]
    [Display(Name = "Cupo maximo")]
    public int CupoMaximo { get; set; }

    [Range(0, 99999.99, ErrorMessage = "El costo no puede ser negativo.")]
    [Display(Name = "Costo de matricula")]
    public decimal CostoMatricula { get; set; }

    public ICollection<Modulo> Modulos { get; set; } = new List<Modulo>();
    public ICollection<CursoDocente> CursoDocentes { get; set; } = new List<CursoDocente>();
    public ICollection<CursoPrerrequisito> Prerrequisitos { get; set; } = new List<CursoPrerrequisito>();
    public ICollection<CursoPrerrequisito> EsPrerrequisitoDe { get; set; } = new List<CursoPrerrequisito>();
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
