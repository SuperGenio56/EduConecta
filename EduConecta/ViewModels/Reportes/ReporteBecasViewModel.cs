using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduConecta.ViewModels.Reportes;

public class FilaBecaViewModel
{
    public string Estudiante { get; set; } = string.Empty;
    public string NombreCampus { get; set; } = string.Empty;
    public string TipoBeca { get; set; } = string.Empty;
    public decimal PorcentajeDescuento { get; set; }
    public int MatriculasBecadas { get; set; }
    public decimal MontoBruto { get; set; }

    public decimal MontoBecado => Math.Round(MontoBruto * PorcentajeDescuento / 100m, 2);
    public decimal MontoAPagar => MontoBruto - MontoBecado;
}

public class ReporteBecasViewModel
{
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public string? TipoBeca { get; set; }
    public int? SedeId { get; set; }

    public SelectList? TiposDeBeca { get; set; }
    public SelectList? Sedes { get; set; }

    public List<FilaBecaViewModel> Filas { get; set; } = new();

    public int TotalMatriculasBecadas => Filas.Sum(f => f.MatriculasBecadas);
    public decimal TotalBruto => Filas.Sum(f => f.MontoBruto);
    public decimal TotalBecado => Filas.Sum(f => f.MontoBecado);
    public decimal TotalAPagar => Filas.Sum(f => f.MontoAPagar);
}
