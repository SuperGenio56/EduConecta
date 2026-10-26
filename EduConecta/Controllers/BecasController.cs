using EduConecta.Data;
using EduConecta.Helpers;
using EduConecta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduConecta.Controllers;

public class BecasController : Controller
{
    private readonly EduConectaContext _context;

    public BecasController(EduConectaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? busqueda, int? estudianteId, int pagina = 1, int? tamanoPagina = null)
    {
        var tamano = OpcionesPaginacion.Normalizar(tamanoPagina);

        if (pagina < 1)
        {
            pagina = 1;
        }

        var consulta = _context.Becas
            .AsNoTracking()
            .Include(b => b.Estudiante)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var texto = busqueda.Trim();
            consulta = consulta.Where(b => b.TipoBeca.Contains(texto));
        }

        if (estudianteId.HasValue)
        {
            consulta = consulta.Where(b => b.EstudianteId == estudianteId.Value);
        }

        consulta = consulta
            .OrderBy(b => b.Estudiante.ApellidoPaterno)
            .ThenBy(b => b.Estudiante.Nombres)
            .ThenBy(b => b.TipoBeca);

        var becas = await ListaPaginada<Beca>.CrearAsync(consulta, pagina, tamano);

        ViewData["Busqueda"] = busqueda;
        ViewData["EstudianteId"] = await CrearListaDeEstudiantesAsync(estudianteId);

        ViewData["FiltrosActuales"] = new Dictionary<string, string>
        {
            ["busqueda"] = busqueda ?? string.Empty,
            ["estudianteId"] = estudianteId?.ToString() ?? string.Empty
        };

        return View(becas);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var beca = await _context.Becas
            .AsNoTracking()
            .Include(b => b.Estudiante)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (beca == null)
        {
            return NotFound();
        }

        return View(beca);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["EstudianteId"] = await CrearListaDeEstudiantesAsync(null);
        return View(new Beca());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Beca beca)
    {
        await ValidarBecaAsync(beca);

        if (!ModelState.IsValid)
        {
            ViewData["EstudianteId"] = await CrearListaDeEstudiantesAsync(beca.EstudianteId);
            return View(beca);
        }

        _context.Becas.Add(beca);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "La beca se registro correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var beca = await _context.Becas.FindAsync(id);

        if (beca == null)
        {
            return NotFound();
        }

        ViewData["EstudianteId"] = await CrearListaDeEstudiantesAsync(beca.EstudianteId);
        return View(beca);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Beca beca)
    {
        if (id != beca.Id)
        {
            return NotFound();
        }

        await ValidarBecaAsync(beca);

        if (!ModelState.IsValid)
        {
            ViewData["EstudianteId"] = await CrearListaDeEstudiantesAsync(beca.EstudianteId);
            return View(beca);
        }

        _context.Update(beca);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "La beca se actualizo correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var beca = await _context.Becas
            .AsNoTracking()
            .Include(b => b.Estudiante)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (beca == null)
        {
            return NotFound();
        }

        return View(beca);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var beca = await _context.Becas.FindAsync(id);

        if (beca == null)
        {
            return NotFound();
        }

        try
        {
            _context.Becas.Remove(beca);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "La beca se elimino correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar la beca porque ya esta aplicada en matriculas.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarBecaAsync(Beca beca)
    {
        var existeElEstudiante = await _context.Estudiantes
            .AnyAsync(e => e.Id == beca.EstudianteId);

        if (!existeElEstudiante)
        {
            ModelState.AddModelError(nameof(beca.EstudianteId),
                "Debe seleccionar un estudiante.");
            return;
        }

        var repetida = await _context.Becas
            .AnyAsync(b => b.Id != beca.Id
                        && b.EstudianteId == beca.EstudianteId
                        && b.TipoBeca == beca.TipoBeca);

        if (repetida)
        {
            ModelState.AddModelError(nameof(beca.TipoBeca),
                "El estudiante ya tiene una beca de ese tipo.");
        }
    }

    private async Task<SelectList> CrearListaDeEstudiantesAsync(int? estudianteId)
    {
        var estudiantes = await _context.Estudiantes
            .AsNoTracking()
            .OrderBy(e => e.ApellidoPaterno)
            .ThenBy(e => e.ApellidoMaterno)
            .ThenBy(e => e.Nombres)
            .ToListAsync();

        var opciones = estudiantes
            .Select(e => new { e.Id, Descripcion = e.NombreCompleto })
            .ToList();

        return new SelectList(opciones, "Id", "Descripcion", estudianteId);
    }
}
