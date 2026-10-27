using EduConecta.Data;
using EduConecta.Helpers;
using EduConecta.Models;
using EduConecta.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduConecta.Controllers;

public class CursosController : Controller
{
    private readonly EduConectaContext _context;

    public CursosController(EduConectaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? busqueda, ModalidadCurso? modalidad, int pagina = 1, int? tamanoPagina = null)
    {
        var tamano = OpcionesPaginacion.Normalizar(tamanoPagina);

        if (pagina < 1)
        {
            pagina = 1;
        }

        var consulta = _context.Cursos
            .AsNoTracking()
            .Include(c => c.CursoDocentes)
                .ThenInclude(cd => cd.Docente)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var texto = busqueda.Trim();

            consulta = consulta.Where(c =>
                c.CodigoCurso.Contains(texto) ||
                c.NombreCurso.Contains(texto));
        }

        if (modalidad.HasValue)
        {
            consulta = consulta.Where(c => c.Modalidad == modalidad.Value);
        }

        consulta = consulta.OrderBy(c => c.CodigoCurso);

        var cursos = await ListaPaginada<Curso>.CrearAsync(consulta, pagina, tamano);

        ViewData["Busqueda"] = busqueda;
        ViewData["Modalidad"] = new SelectList(Enum.GetValues<ModalidadCurso>(), modalidad);

        ViewData["FiltrosActuales"] = new Dictionary<string, string>
        {
            ["busqueda"] = busqueda ?? string.Empty,
            ["modalidad"] = modalidad?.ToString() ?? string.Empty
        };

        return View(cursos);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var curso = await _context.Cursos
            .AsNoTracking()
            .Include(c => c.CursoDocentes)
                .ThenInclude(cd => cd.Docente)
            .Include(c => c.Prerrequisitos)
                .ThenInclude(cp => cp.Prerrequisito)
            .Include(c => c.EsPrerrequisitoDe)
                .ThenInclude(cp => cp.Curso)
            .Include(c => c.Modulos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (curso == null)
        {
            return NotFound();
        }

        return View(curso);
    }

    public async Task<IActionResult> Create()
    {
        var formulario = new CursoFormViewModel();
        await CargarListasAsync(formulario);
        return View(formulario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CursoFormViewModel formulario)
    {
        await ValidarFormularioAsync(formulario);

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(formulario);
            return View(formulario);
        }

        var curso = new Curso();
        CopiarDatos(formulario, curso);
        AgregarRelaciones(formulario, curso);

        _context.Cursos.Add(curso);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "El curso se registro correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var curso = await _context.Cursos
            .AsNoTracking()
            .Include(c => c.CursoDocentes)
            .Include(c => c.Prerrequisitos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (curso == null)
        {
            return NotFound();
        }

        var formulario = new CursoFormViewModel
        {
            Id = curso.Id,
            CodigoCurso = curso.CodigoCurso,
            NombreCurso = curso.NombreCurso,
            Creditos = curso.Creditos,
            Modalidad = curso.Modalidad,
            CupoMaximo = curso.CupoMaximo,
            CostoMatricula = curso.CostoMatricula,
            DocentesSeleccionados = curso.CursoDocentes.Select(cd => cd.DocenteId).ToList(),
            PrerrequisitosSeleccionados = curso.Prerrequisitos.Select(cp => cp.PrerrequisitoId).ToList()
        };

        await CargarListasAsync(formulario);
        return View(formulario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CursoFormViewModel formulario)
    {
        if (id != formulario.Id)
        {
            return NotFound();
        }

        await ValidarFormularioAsync(formulario);

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(formulario);
            return View(formulario);
        }

        var curso = await _context.Cursos
            .Include(c => c.CursoDocentes)
            .Include(c => c.Prerrequisitos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (curso == null)
        {
            return NotFound();
        }

        CopiarDatos(formulario, curso);

        _context.CursosDocentes.RemoveRange(curso.CursoDocentes);
        _context.CursosPrerrequisitos.RemoveRange(curso.Prerrequisitos);

        AgregarRelaciones(formulario, curso);

        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "El curso se actualizo correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var curso = await _context.Cursos
            .AsNoTracking()
            .Include(c => c.CursoDocentes)
                .ThenInclude(cd => cd.Docente)
            .Include(c => c.Modulos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (curso == null)
        {
            return NotFound();
        }

        return View(curso);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);

        if (curso == null)
        {
            return NotFound();
        }

        try
        {
            _context.Cursos.Remove(curso);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "El curso se elimino correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar el curso porque es prerrequisito de otros cursos o tiene matriculas.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarFormularioAsync(CursoFormViewModel formulario)
    {
        if (formulario.DocentesSeleccionados.Count == 0)
        {
            ModelState.AddModelError(nameof(formulario.DocentesSeleccionados),
                "Debe seleccionar al menos un docente.");
        }

        if (formulario.PrerrequisitosSeleccionados.Contains(formulario.Id))
        {
            ModelState.AddModelError(nameof(formulario.PrerrequisitosSeleccionados),
                "Un curso no puede ser prerrequisito de si mismo.");
        }

        var codigoRepetido = await _context.Cursos
            .AnyAsync(c => c.Id != formulario.Id && c.CodigoCurso == formulario.CodigoCurso);

        if (codigoRepetido)
        {
            ModelState.AddModelError(nameof(formulario.CodigoCurso),
                "Ya existe un curso con ese codigo.");
        }
    }

    private static void CopiarDatos(CursoFormViewModel formulario, Curso curso)
    {
        curso.CodigoCurso = formulario.CodigoCurso.Trim().ToUpper();
        curso.NombreCurso = formulario.NombreCurso.Trim();
        curso.Creditos = formulario.Creditos;
        curso.Modalidad = formulario.Modalidad;
        curso.CupoMaximo = formulario.CupoMaximo;
        curso.CostoMatricula = formulario.CostoMatricula;
    }

    private static void AgregarRelaciones(CursoFormViewModel formulario, Curso curso)
    {
        foreach (var docenteId in formulario.DocentesSeleccionados.Distinct())
        {
            curso.CursoDocentes.Add(new CursoDocente { DocenteId = docenteId });
        }

        foreach (var prerrequisitoId in formulario.PrerrequisitosSeleccionados.Distinct())
        {
            curso.Prerrequisitos.Add(new CursoPrerrequisito { PrerrequisitoId = prerrequisitoId });
        }
    }

    private async Task CargarListasAsync(CursoFormViewModel formulario)
    {
        formulario.Docentes = new MultiSelectList(
            await _context.Docentes.AsNoTracking().OrderBy(d => d.NombreCompleto).ToListAsync(),
            "Id", "NombreCompleto", formulario.DocentesSeleccionados);

        formulario.CursosDisponibles = new MultiSelectList(
            await _context.Cursos.AsNoTracking()
                .Where(c => c.Id != formulario.Id)
                .OrderBy(c => c.NombreCurso)
                .ToListAsync(),
            "Id", "NombreCurso", formulario.PrerrequisitosSeleccionados);
    }
}
