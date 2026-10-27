using EduConecta.Data;
using EduConecta.Helpers;
using EduConecta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduConecta.Controllers;

public class DocentesController : Controller
{
    private readonly EduConectaContext _context;

    public DocentesController(EduConectaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? busqueda, int pagina = 1, int? tamanoPagina = null)
    {
        var tamano = OpcionesPaginacion.Normalizar(tamanoPagina);

        if (pagina < 1)
        {
            pagina = 1;
        }

        var consulta = _context.Docentes
            .AsNoTracking()
            .Include(d => d.CursoDocentes)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var texto = busqueda.Trim();

            consulta = consulta.Where(d =>
                d.NombreCompleto.Contains(texto) ||
                d.Especialidad.Contains(texto) ||
                d.Correo.Contains(texto));
        }

        consulta = consulta.OrderBy(d => d.NombreCompleto);

        var docentes = await ListaPaginada<Docente>.CrearAsync(consulta, pagina, tamano);

        ViewData["Busqueda"] = busqueda;

        ViewData["FiltrosActuales"] = new Dictionary<string, string>
        {
            ["busqueda"] = busqueda ?? string.Empty
        };

        return View(docentes);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var docente = await _context.Docentes
            .AsNoTracking()
            .Include(d => d.CursoDocentes)
                .ThenInclude(cd => cd.Curso)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (docente == null)
        {
            return NotFound();
        }

        return View(docente);
    }

    public IActionResult Create()
    {
        return View(new Docente());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Docente docente)
    {
        await ValidarCorreoUnicoAsync(docente);

        if (!ModelState.IsValid)
        {
            return View(docente);
        }

        _context.Docentes.Add(docente);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "El docente se registro correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var docente = await _context.Docentes.FindAsync(id);

        if (docente == null)
        {
            return NotFound();
        }

        return View(docente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Docente docente)
    {
        if (id != docente.Id)
        {
            return NotFound();
        }

        await ValidarCorreoUnicoAsync(docente);

        if (!ModelState.IsValid)
        {
            return View(docente);
        }

        _context.Update(docente);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "El docente se actualizo correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var docente = await _context.Docentes
            .AsNoTracking()
            .Include(d => d.CursoDocentes)
                .ThenInclude(cd => cd.Curso)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (docente == null)
        {
            return NotFound();
        }

        return View(docente);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var docente = await _context.Docentes.FindAsync(id);

        if (docente == null)
        {
            return NotFound();
        }

        _context.Docentes.Remove(docente);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "El docente se elimino correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarCorreoUnicoAsync(Docente docente)
    {
        var repetido = await _context.Docentes
            .AnyAsync(d => d.Id != docente.Id && d.Correo == docente.Correo);

        if (repetido)
        {
            ModelState.AddModelError(nameof(docente.Correo),
                "Ya existe un docente con ese correo.");
        }
    }
}
