using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduConecta.Models;

public class Estudiante
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
    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El pais de la direccion es obligatorio.")]
    [StringLength(40)]
    [Display(Name = "Pais")]
    public string DireccionPais { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad de la direccion es obligatoria.")]
    [StringLength(60)]
    [Display(Name = "Ciudad")]
    public string DireccionCiudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La calle es obligatoria.")]
    [StringLength(120)]
    [Display(Name = "Calle")]
    public string DireccionCalle { get; set; } = string.Empty;

    [Display(Name = "Sede")]
    public int SedeId { get; set; }

    public Sede Sede { get; set; } = null!;

    public ICollection<CorreoEstudiante> Correos { get; set; } = new List<CorreoEstudiante>();
    public ICollection<TelefonoEstudiante> Telefonos { get; set; } = new List<TelefonoEstudiante>();
    public ICollection<Beca> Becas { get; set; } = new List<Beca>();
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    [NotMapped]
    [Display(Name = "Estudiante")]
    public string NombreCompleto =>
        string.IsNullOrWhiteSpace(ApellidoMaterno)
            ? $"{ApellidoPaterno}, {Nombres}"
            : $"{ApellidoPaterno} {ApellidoMaterno}, {Nombres}";

    [NotMapped]
    [Display(Name = "Edad")]
    public int Edad
    {
        get
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - FechaNacimiento.Year;

            if (FechaNacimiento.Date > hoy.AddYears(-edad))
            {
                edad--;
            }

            return edad;
        }
    }

    [NotMapped]
    [Display(Name = "Direccion")]
    public string DireccionCompleta => $"{DireccionCalle}, {DireccionCiudad}, {DireccionPais}";
}
