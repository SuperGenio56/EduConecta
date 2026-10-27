using EduConecta.Data;
using EduConecta.Helpers;
using EduConecta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduConecta.Controllers;

public class MatriculasController : Controller
{
    private readonly EduConectaContext _context;

    public MatriculasController(EduConectaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? periodoAcademico, int? cursoId, EstadoMatricula? estado,
        int pagina = 1, int? tamanoPagina = null)
    {
        var tamano = OpcionesPaginacion.Normalizar(tamanoPagina);

        if (pagina < 1)
        {
            pagina = 1;
        }

        var consulta = _context.Matriculas
            .AsNoTracking()
            .Include(m => m.Estudiante)
            .Include(m => m.Curso)
            .Include(m => m.Beca)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(periodoAcademico))
        {
            consulta = consulta.Where(m => m.PeriodoAcademico == periodoAcademico);
        }

        if (cursoId.HasValue)
        {
            consulta = consulta.Where(m => m.CursoId == cursoId.Value);
        }

        if (estado.HasValue)
        {
            consulta = consulta.Where(m => m.Estado == estado.Value);
        }

        consulta = consulta
            .OrderByDescending(m => m.FechaMatricula)
            .ThenBy(m => m.Id);

        var matriculas = await ListaPaginada<Matricula>.CrearAsync(consulta, pagina, tamano);

        var periodos = await _context.Matriculas
            .AsNoTracking()
            .Select(m => m.PeriodoAcademico)
            .Distinct()
            .OrderByDescending(p => p)
            .ToListAsync();

        ViewData["PeriodoAcademico"] = new SelectList(periodos, periodoAcademico);
        ViewData["CursoId"] = await CrearListaDeCursosAsync(cursoId);
        ViewData["Estado"] = new SelectList(Enum.GetValues<EstadoMatricula>(), estado);

        ViewData["FiltrosActuales"] = new Dictionary<string, string>
        {
            ["periodoAcademico"] = periodoAcademico ?? string.Empty,
            ["cursoId"] = cursoId?.ToString() ?? string.Empty,
            ["estado"] = estado?.ToString() ?? string.Empty
        };

        return View(matriculas);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var matricula = await BuscarConRelacionesAsync(id.Value);

        if (matricula == null)
        {
            return NotFound();
        }

        return View(matricula);
    }

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync(null, null, null);
        return View(new Matricula { FechaMatricula = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Matricula matricula)
    {
        await ValidarMatriculaAsync(matricula);

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(matricula.EstudianteId, matricula.CursoId, matricula.BecaId);
            return View(matricula);
        }

        _context.Matriculas.Add(matricula);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "La matricula se registro correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var matricula = await _context.Matriculas.FindAsync(id);

        if (matricula == null)
        {
            return NotFound();
        }

        await CargarListasAsync(matricula.EstudianteId, matricula.CursoId, matricula.BecaId);
        return View(matricula);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Matricula matricula)
    {
        if (id != matricula.Id)
        {
            return NotFound();
        }

        await ValidarMatriculaAsync(matricula);

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(matricula.EstudianteId, matricula.CursoId, matricula.BecaId);
            return View(matricula);
        }

        _context.Update(matricula);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "La matricula se actualizo correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var matricula = await BuscarConRelacionesAsync(id.Value);

        if (matricula == null)
        {
            return NotFound();
        }

        return View(matricula);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var matricula = await _context.Matriculas.FindAsync(id);

        if (matricula == null)
        {
            return NotFound();
        }

        try
        {
            _context.Matriculas.Remove(matricula);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "La matricula se elimino correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se pudo eliminar la matricula.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<Matricula?> BuscarConRelacionesAsync(int id)
    {
        return await _context.Matriculas
            .AsNoTracking()
            .Include(m => m.Estudiante)
            .Include(m => m.Curso)
            .Include(m => m.Beca)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    private async Task ValidarMatriculaAsync(Matricula matricula)
    {
        var duplicada = await _context.Matriculas
            .AnyAsync(m => m.Id != matricula.Id
                        && m.EstudianteId == matricula.EstudianteId
                        && m.CursoId == matricula.CursoId
                        && m.PeriodoAcademico == matricula.PeriodoAcademico);

        if (duplicada)
        {
            ModelState.AddModelError(string.Empty,
                "El estudiante ya esta matriculado en ese curso durante el mismo periodo.");
        }

        var curso = await _context.Cursos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == matricula.CursoId);

        if (curso == null)
        {
            ModelState.AddModelError(nameof(matricula.CursoId), "Debe seleccionar un curso.");
        }
        else
        {
            var inscritos = await _context.Matriculas
                .CountAsync(m => m.Id != matricula.Id
                              && m.CursoId == matricula.CursoId
                              && m.PeriodoAcademico == matricula.PeriodoAcademico
                              && m.Estado != EstadoMatricula.Anulada);

            if (inscritos >= curso.CupoMaximo)
            {
                ModelState.AddModelError(nameof(matricula.CursoId),
                    $"El curso ya alcanzo su cupo maximo de {curso.CupoMaximo} estudiantes.");
            }
        }

        if (matricula.BecaId.HasValue)
        {
            var becaDelEstudiante = await _context.Becas
                .AnyAsync(b => b.Id == matricula.BecaId.Value
                            && b.EstudianteId == matricula.EstudianteId);

            if (!becaDelEstudiante)
            {
                ModelState.AddModelError(nameof(matricula.BecaId),
                    "La beca seleccionada no pertenece a ese estudiante.");
            }
        }

        var prerrequisitos = await _context.CursosPrerrequisitos
            .Where(cp => cp.CursoId == matricula.CursoId)
            .Select(cp => cp.PrerrequisitoId)
            .ToListAsync();

        if (prerrequisitos.Count > 0)
        {
            var aprobados = await _context.Matriculas
                .Where(m => m.EstudianteId == matricula.EstudianteId
                         && m.Estado == EstadoMatricula.Aprobada
                         && prerrequisitos.Contains(m.CursoId))
                .Select(m => m.CursoId)
                .ToListAsync();

            var faltantes = await _context.Cursos
                .Where(c => prerrequisitos.Contains(c.Id) && !aprobados.Contains(c.Id))
                .Select(c => c.NombreCurso)
                .ToListAsync();

            if (faltantes.Count > 0)
            {
                ModelState.AddModelError(nameof(matricula.CursoId),
                    "El estudiante todavia no aprueba: " + string.Join(", ", faltantes) + ".");
            }
        }
    }

    private async Task CargarListasAsync(int? estudianteId, int? cursoId, int? becaId)
    {
        var estudiantes = await _context.Estudiantes
            .AsNoTracking()
            .OrderBy(e => e.ApellidoPaterno)
            .ThenBy(e => e.ApellidoMaterno)
            .ThenBy(e => e.Nombres)
            .ToListAsync();

        ViewData["EstudianteId"] = new SelectList(
            estudiantes.Select(e => new { e.Id, Descripcion = e.NombreCompleto }),
            "Id", "Descripcion", estudianteId);

        ViewData["CursoId"] = await CrearListaDeCursosAsync(cursoId);

        var becas = await _context.Becas
            .AsNoTracking()
            .Include(b => b.Estudiante)
            .Where(b => estudianteId == null || b.EstudianteId == estudianteId)
            .OrderBy(b => b.TipoBeca)
            .ToListAsync();

        ViewData["BecaId"] = new SelectList(
            becas.Select(b => new
            {
                b.Id,
                Descripcion = $"{b.TipoBeca} ({b.PorcentajeDescuento:N2} %) - {b.Estudiante.NombreCompleto}"
            }),
            "Id", "Descripcion", becaId);
    }

    private async Task<SelectList> CrearListaDeCursosAsync(int? cursoId)
    {
        var cursos = await _context.Cursos
            .AsNoTracking()
            .OrderBy(c => c.NombreCurso)
            .Select(c => new { c.Id, Descripcion = c.CodigoCurso + " - " + c.NombreCurso })
            .ToListAsync();

        return new SelectList(cursos, "Id", "Descripcion", cursoId);
    }
}
