using EduConecta.Data;
using EduConecta.Models;
using EduConecta.ViewModels.Reportes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduConecta.Controllers;

public class ReportesController : Controller
{
    private readonly EduConectaContext _context;

    public ReportesController(EduConectaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OcupacionPorCurso(
        string? periodoAcademico, ModalidadCurso? modalidad, int? sedeId)
    {
        var consulta = _context.Matriculas
            .AsNoTracking()
            .Where(m => m.Estado != EstadoMatricula.Anulada);

        if (!string.IsNullOrWhiteSpace(periodoAcademico))
        {
            consulta = consulta.Where(m => m.PeriodoAcademico == periodoAcademico);
        }

        if (modalidad.HasValue)
        {
            consulta = consulta.Where(m => m.Curso.Modalidad == modalidad.Value);
        }

        if (sedeId.HasValue)
        {
            consulta = consulta.Where(m => m.Estudiante.SedeId == sedeId.Value);
        }

        var filas = await consulta
            .GroupBy(m => new
            {
                m.Curso.CodigoCurso,
                m.Curso.NombreCurso,
                m.Curso.Modalidad,
                m.Curso.CupoMaximo
            })
            .OrderBy(g => g.Key.NombreCurso)
            .Select(g => new FilaOcupacionViewModel
            {
                CodigoCurso = g.Key.CodigoCurso,
                NombreCurso = g.Key.NombreCurso,
                Modalidad = g.Key.Modalidad,
                CupoMaximo = g.Key.CupoMaximo,
                Matriculados = g.Count(),
                MontoBruto = g.Sum(m => m.Curso.CostoMatricula),
                MontoDescuento = g.Sum(m => m.Beca == null
                    ? 0m
                    : m.Curso.CostoMatricula * m.Beca.PorcentajeDescuento / 100m)
            })
            .ToListAsync();

        var periodos = await _context.Matriculas
            .AsNoTracking()
            .Select(m => m.PeriodoAcademico)
            .Distinct()
            .OrderByDescending(p => p)
            .ToListAsync();

        var modelo = new ReporteOcupacionViewModel
        {
            PeriodoAcademico = periodoAcademico,
            Modalidad = modalidad,
            SedeId = sedeId,
            Filas = filas,
            Periodos = new SelectList(periodos, periodoAcademico),
            Sedes = await CrearListaDeSedesAsync(sedeId)
        };

        return View(modelo);
    }

    public async Task<IActionResult> BecasAplicadas(
        DateTime? desde, DateTime? hasta, string? tipoBeca, int? sedeId)
    {
        var filas = await _context.Becas
            .AsNoTracking()
            .Where(b => string.IsNullOrEmpty(tipoBeca) || b.TipoBeca == tipoBeca)
            .Where(b => sedeId == null || b.Estudiante.SedeId == sedeId)
            .OrderBy(b => b.Estudiante.ApellidoPaterno)
            .ThenBy(b => b.Estudiante.Nombres)
            .Select(b => new FilaBecaViewModel
            {
                Estudiante = b.Estudiante.ApellidoMaterno == null
                    ? b.Estudiante.ApellidoPaterno + ", " + b.Estudiante.Nombres
                    : b.Estudiante.ApellidoPaterno + " " + b.Estudiante.ApellidoMaterno + ", " + b.Estudiante.Nombres,
                NombreCampus = b.Estudiante.Sede.NombreCampus,
                TipoBeca = b.TipoBeca,
                PorcentajeDescuento = b.PorcentajeDescuento,
                MatriculasBecadas = b.Matriculas.Count(m =>
                    m.Estado != EstadoMatricula.Anulada &&
                    (desde == null || m.FechaMatricula >= desde) &&
                    (hasta == null || m.FechaMatricula <= hasta)),
                MontoBruto = b.Matriculas
                    .Where(m => m.Estado != EstadoMatricula.Anulada &&
                                (desde == null || m.FechaMatricula >= desde) &&
                                (hasta == null || m.FechaMatricula <= hasta))
                    .Sum(m => (decimal?)m.Curso.CostoMatricula) ?? 0m
            })
            .ToListAsync();

        filas = filas.Where(f => f.MatriculasBecadas > 0).ToList();

        var tipos = await _context.Becas
            .AsNoTracking()
            .Select(b => b.TipoBeca)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        var modelo = new ReporteBecasViewModel
        {
            Desde = desde,
            Hasta = hasta,
            TipoBeca = tipoBeca,
            SedeId = sedeId,
            Filas = filas,
            TiposDeBeca = new SelectList(tipos, tipoBeca),
            Sedes = await CrearListaDeSedesAsync(sedeId)
        };

        return View(modelo);
    }

    private async Task<SelectList> CrearListaDeSedesAsync(int? sedeId)
    {
        var sedes = await _context.Sedes
            .AsNoTracking()
            .OrderBy(s => s.NombreCampus)
            .ToListAsync();

        return new SelectList(sedes, "Id", "NombreCampus", sedeId);
    }
}
