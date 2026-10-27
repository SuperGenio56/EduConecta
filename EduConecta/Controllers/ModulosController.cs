using EduConecta.Data;
using EduConecta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduConecta.Controllers;

public class ModulosController : Controller
{
    private readonly EduConectaContext _context;

    public ModulosController(EduConectaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int cursoId)
    {
        var curso = await _context.Cursos
            .AsNoTracking()
            .Include(c => c.Modulos.OrderBy(m => m.NumeroModulo))
            .FirstOrDefaultAsync(c => c.Id == cursoId);

        if (curso == null)
        {
            return NotFound();
        }

        ViewData["Curso"] = curso.NombreCurso;
        ViewData["CursoId"] = curso.Id;

        return View(curso.Modulos.ToList());
    }

    public async Task<IActionResult> Create(int cursoId)
    {
        var curso = await _context.Cursos
            .AsNoTracking()
            .Include(c => c.Modulos)
            .FirstOrDefaultAsync(c => c.Id == cursoId);

        if (curso == null)
        {
            return NotFound();
        }

        ViewData["Curso"] = curso.NombreCurso;

        var siguienteNumero = curso.Modulos.Any() ? curso.Modulos.Max(m => m.NumeroModulo) + 1 : 1;

        return View(new Modulo { CursoId = cursoId, NumeroModulo = siguienteNumero });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Modulo modulo)
    {
        var numeroRepetido = await _context.Modulos
            .AnyAsync(m => m.CursoId == modulo.CursoId && m.NumeroModulo == modulo.NumeroModulo);

        if (numeroRepetido)
        {
            ModelState.AddModelError(nameof(modulo.NumeroModulo),
                "Ese numero de modulo ya existe en el curso.");
        }

        if (!ModelState.IsValid)
        {
            ViewData["Curso"] = await ObtenerNombreDelCursoAsync(modulo.CursoId);
            return View(modulo);
        }

        _context.Modulos.Add(modulo);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "El modulo se registro correctamente.";
        return RedirectToAction(nameof(Index), new { cursoId = modulo.CursoId });
    }

    public async Task<IActionResult> Edit(int cursoId, int numeroModulo)
    {
        var modulo = await _context.Modulos.FindAsync(cursoId, numeroModulo);

        if (modulo == null)
        {
            return NotFound();
        }

        ViewData["Curso"] = await ObtenerNombreDelCursoAsync(cursoId);
        return View(modulo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int cursoId, int numeroModulo, Modulo modulo)
    {
        if (cursoId != modulo.CursoId || numeroModulo != modulo.NumeroModulo)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Curso"] = await ObtenerNombreDelCursoAsync(cursoId);
            return View(modulo);
        }

        _context.Update(modulo);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "El modulo se actualizo correctamente.";
        return RedirectToAction(nameof(Index), new { cursoId });
    }

    public async Task<IActionResult> Delete(int cursoId, int numeroModulo)
    {
        var modulo = await _context.Modulos
            .AsNoTracking()
            .Include(m => m.Curso)
            .FirstOrDefaultAsync(m => m.CursoId == cursoId && m.NumeroModulo == numeroModulo);

        if (modulo == null)
        {
            return NotFound();
        }

        return View(modulo);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int cursoId, int numeroModulo)
    {
        var modulo = await _context.Modulos.FindAsync(cursoId, numeroModulo);

        if (modulo == null)
        {
            return NotFound();
        }

        _context.Modulos.Remove(modulo);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "El modulo se elimino correctamente.";
        return RedirectToAction(nameof(Index), new { cursoId });
    }

    private async Task<string?> ObtenerNombreDelCursoAsync(int cursoId)
    {
        return await _context.Cursos
            .Where(c => c.Id == cursoId)
            .Select(c => c.NombreCurso)
            .FirstOrDefaultAsync();
    }
}
