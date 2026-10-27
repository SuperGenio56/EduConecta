using System.ComponentModel.DataAnnotations;
using EduConecta.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduConecta.ViewModels;

public class CursoFormViewModel
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
    public int Creditos { get; set; } = 4;

    public ModalidadCurso Modalidad { get; set; }

    [Range(1, 500, ErrorMessage = "El cupo maximo debe estar entre 1 y 500.")]
    [Display(Name = "Cupo maximo")]
    public int CupoMaximo { get; set; } = 30;

    [Range(0, 99999.99, ErrorMessage = "El costo no puede ser negativo.")]
    [Display(Name = "Costo de matricula")]
    public decimal CostoMatricula { get; set; }

    [Display(Name = "Docentes que lo imparten")]
    public List<int> DocentesSeleccionados { get; set; } = new();

    [Display(Name = "Cursos que debe aprobar antes")]
    public List<int> PrerrequisitosSeleccionados { get; set; } = new();

    public MultiSelectList? Docentes { get; set; }
    public MultiSelectList? CursosDisponibles { get; set; }
}
