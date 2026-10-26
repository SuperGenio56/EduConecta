using EduConecta.Data;
using EduConecta.Helpers;
using EduConecta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduConecta.Controllers;

public class SedesController : Controller
{
    private readonly EduConectaContext _context;

    public SedesController(EduConectaContext context)
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

        var consulta = _context.Sedes
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var texto = busqueda.Trim();

            consulta = consulta.Where(s =>
                s.NombreCampus.Contains(texto) ||
                s.Ciudad.Contains(texto) ||
                s.Pais.Contains(texto));
        }

        consulta = consulta
            .OrderBy(s => s.Pais)
            .ThenBy(s => s.Ciudad)
            .ThenBy(s => s.NombreCampus);

        var sedes = await ListaPaginada<Sede>.CrearAsync(consulta, pagina, tamano);

        ViewData["Busqueda"] = busqueda;

        ViewData["FiltrosActuales"] = new Dictionary<string, string>
        {
            ["busqueda"] = busqueda ?? string.Empty
        };

        return View(sedes);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sede = await _context.Sedes
            .AsNoTracking()
            .Include(s => s.Estudiantes)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sede == null)
        {
            return NotFound();
        }

        return View(sede);
    }

    public IActionResult Create()
    {
        return View(new Sede { Pais = "Bolivia" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Sede sede)
    {
        await ValidarCampusUnicoAsync(sede);

        if (!ModelState.IsValid)
        {
            return View(sede);
        }

        _context.Sedes.Add(sede);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "La sede se registro correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sede = await _context.Sedes.FindAsync(id);

        if (sede == null)
        {
            return NotFound();
        }

        return View(sede);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Sede sede)
    {
        if (id != sede.Id)
        {
            return NotFound();
        }

        await ValidarCampusUnicoAsync(sede);

        if (!ModelState.IsValid)
        {
            return View(sede);
        }

        _context.Update(sede);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "La sede se actualizo correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sede = await _context.Sedes
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sede == null)
        {
            return NotFound();
        }

        return View(sede);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sede = await _context.Sedes.FindAsync(id);

        if (sede == null)
        {
            return NotFound();
        }

        try
        {
            _context.Sedes.Remove(sede);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "La sede se elimino correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar la sede porque tiene estudiantes registrados.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarCampusUnicoAsync(Sede sede)
    {
        var repetido = await _context.Sedes
            .AnyAsync(s => s.Id != sede.Id && s.NombreCampus == sede.NombreCampus);

        if (repetido)
        {
            ModelState.AddModelError(nameof(sede.NombreCampus),
                "Ya existe una sede con ese nombre de campus.");
        }
    }
}
