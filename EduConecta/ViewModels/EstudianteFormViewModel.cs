using System.ComponentModel.DataAnnotations;

namespace EduConecta.ViewModels;

public class EstudianteFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(60)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
    [StringLength(40)]
    [Display(Name = "Apellido paterno")]
    public string ApellidoPaterno { get; set; } = string.Empty;

    [StringLength(40)]
    [Display(Name = "Apellido materno")]
    public string? ApellidoMaterno { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateTime FechaNacimiento { get; set; } = new DateTime(2005, 1, 1);

    [Required(ErrorMessage = "El pais de la direccion es obligatorio.")]
    [StringLength(40)]
    [Display(Name = "Pais")]
    public string DireccionPais { get; set; } = "Bolivia";

    [Required(ErrorMessage = "La ciudad de la direccion es obligatoria.")]
    [StringLength(60)]
    [Display(Name = "Ciudad")]
    public string DireccionCiudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La calle es obligatoria.")]
    [StringLength(120)]
    [Display(Name = "Calle")]
    public string DireccionCalle { get; set; } = string.Empty;

    [Display(Name = "Sede")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una sede.")]
    public int SedeId { get; set; }

    [Display(Name = "Correos (uno por linea)")]
    public string? Correos { get; set; }

    [Display(Name = "Telefonos (uno por linea)")]
    public string? Telefonos { get; set; }
}
