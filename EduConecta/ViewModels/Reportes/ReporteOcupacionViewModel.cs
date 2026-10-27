using EduConecta.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduConecta.ViewModels.Reportes;

public class FilaOcupacionViewModel
{
    public string CodigoCurso { get; set; } = string.Empty;
    public string NombreCurso { get; set; } = string.Empty;
    public ModalidadCurso Modalidad { get; set; }
    public int CupoMaximo { get; set; }
    public int Matriculados { get; set; }
    public decimal MontoBruto { get; set; }
    public decimal MontoDescuento { get; set; }

    public int CuposDisponibles => CupoMaximo - Matriculados;

    public decimal PorcentajeOcupacion =>
        CupoMaximo == 0 ? 0m : Math.Round(Matriculados * 100m / CupoMaximo, 1);

    public decimal MontoNeto => MontoBruto - MontoDescuento;
}

public class ReporteOcupacionViewModel
{
    public string? PeriodoAcademico { get; set; }
    public ModalidadCurso? Modalidad { get; set; }
    public int? SedeId { get; set; }

    public SelectList? Periodos { get; set; }
    public SelectList? Sedes { get; set; }

    public List<FilaOcupacionViewModel> Filas { get; set; } = new();

    public int TotalMatriculados => Filas.Sum(f => f.Matriculados);
    public int TotalCupos => Filas.Sum(f => f.CupoMaximo);
    public decimal TotalBruto => Filas.Sum(f => f.MontoBruto);
    public decimal TotalDescuento => Filas.Sum(f => f.MontoDescuento);
    public decimal TotalNeto => Filas.Sum(f => f.MontoNeto);
}
