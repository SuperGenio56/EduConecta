using System.ComponentModel.DataAnnotations;
using EduConecta.Data;
using EduConecta.Helpers;
using EduConecta.Models;
using EduConecta.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduConecta.Controllers;

public class EstudiantesController : Controller
{
    private readonly EduConectaContext _context;

    public EstudiantesController(EduConectaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? busqueda, int? sedeId, int pagina = 1, int? tamanoPagina = null)
    {
        var tamano = OpcionesPaginacion.Normalizar(tamanoPagina);

        if (pagina < 1)
        {
            pagina = 1;
        }

        var consulta = _context.Estudiantes
            .AsNoTracking()
            .Include(e => e.Sede)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var texto = busqueda.Trim();

            consulta = consulta.Where(e =>
                e.Nombres.Contains(texto) ||
                e.ApellidoPaterno.Contains(texto) ||
                (e.ApellidoMaterno != null && e.ApellidoMaterno.Contains(texto)));
        }

        if (sedeId.HasValue)
        {
            consulta = consulta.Where(e => e.SedeId == sedeId.Value);
        }

        consulta = consulta
            .OrderBy(e => e.ApellidoPaterno)
            .ThenBy(e => e.ApellidoMaterno)
            .ThenBy(e => e.Nombres);

        var estudiantes = await ListaPaginada<Estudiante>.CrearAsync(consulta, pagina, tamano);

        ViewData["Busqueda"] = busqueda;
        ViewData["SedeId"] = await CrearListaDeSedesAsync(sedeId);

        ViewData["FiltrosActuales"] = new Dictionary<string, string>
        {
            ["busqueda"] = busqueda ?? string.Empty,
            ["sedeId"] = sedeId?.ToString() ?? string.Empty
        };

        return View(estudiantes);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var estudiante = await _context.Estudiantes
            .AsNoTracking()
            .Include(e => e.Sede)
            .Include(e => e.Correos)
            .Include(e => e.Telefonos)
            .Include(e => e.Becas)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (estudiante == null)
        {
            return NotFound();
        }

        return View(estudiante);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["SedeId"] = await CrearListaDeSedesAsync(null);
        return View(new EstudianteFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EstudianteFormViewModel formulario)
    {
        var correos = SepararLineas(formulario.Correos);
        var telefonos = SepararLineas(formulario.Telefonos);

        await ValidarContactosAsync(0, correos, telefonos);

        if (!ModelState.IsValid)
        {
            ViewData["SedeId"] = await CrearListaDeSedesAsync(formulario.SedeId);
            return View(formulario);
        }

        var estudiante = new Estudiante();
        CopiarDatos(formulario, estudiante);

        foreach (var correo in correos)
        {
            estudiante.Correos.Add(new CorreoEstudiante { Direccion = correo });
        }

        foreach (var telefono in telefonos)
        {
            estudiante.Telefonos.Add(new TelefonoEstudiante { Numero = telefono });
        }

        _context.Estudiantes.Add(estudiante);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "El estudiante se registro correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var estudiante = await _context.Estudiantes
            .AsNoTracking()
            .Include(e => e.Correos)
            .Include(e => e.Telefonos)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (estudiante == null)
        {
            return NotFound();
        }

        var formulario = new EstudianteFormViewModel
        {
            Id = estudiante.Id,
            Nombres = estudiante.Nombres,
            ApellidoPaterno = estudiante.ApellidoPaterno,
            ApellidoMaterno = estudiante.ApellidoMaterno,
            FechaNacimiento = estudiante.FechaNacimiento,
            DireccionPais = estudiante.DireccionPais,
            DireccionCiudad = estudiante.DireccionCiudad,
            DireccionCalle = estudiante.DireccionCalle,
            SedeId = estudiante.SedeId,
            Correos = string.Join(Environment.NewLine, estudiante.Correos.Select(c => c.Direccion)),
            Telefonos = string.Join(Environment.NewLine, estudiante.Telefonos.Select(t => t.Numero))
        };

        ViewData["SedeId"] = await CrearListaDeSedesAsync(estudiante.SedeId);
        return View(formulario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EstudianteFormViewModel formulario)
    {
        if (id != formulario.Id)
        {
            return NotFound();
        }

        var correos = SepararLineas(formulario.Correos);
        var telefonos = SepararLineas(formulario.Telefonos);

        await ValidarContactosAsync(id, correos, telefonos);

        if (!ModelState.IsValid)
        {
            ViewData["SedeId"] = await CrearListaDeSedesAsync(formulario.SedeId);
            return View(formulario);
        }

        var estudiante = await _context.Estudiantes
            .Include(e => e.Correos)
            .Include(e => e.Telefonos)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (estudiante == null)
        {
            return NotFound();
        }

        CopiarDatos(formulario, estudiante);

        _context.CorreosEstudiante.RemoveRange(estudiante.Correos);
        _context.TelefonosEstudiante.RemoveRange(estudiante.Telefonos);

        foreach (var correo in correos)
        {
            estudiante.Correos.Add(new CorreoEstudiante { Direccion = correo });
        }

        foreach (var telefono in telefonos)
        {
            estudiante.Telefonos.Add(new TelefonoEstudiante { Numero = telefono });
        }

        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "El estudiante se actualizo correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var estudiante = await _context.Estudiantes
            .AsNoTracking()
            .Include(e => e.Sede)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (estudiante == null)
        {
            return NotFound();
        }

        return View(estudiante);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var estudiante = await _context.Estudiantes.FindAsync(id);

        if (estudiante == null)
        {
            return NotFound();
        }

        try
        {
            _context.Estudiantes.Remove(estudiante);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "El estudiante se elimino correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar el estudiante porque tiene becas registradas.";
        }

        return RedirectToAction(nameof(Index));
    }

    private static void CopiarDatos(EstudianteFormViewModel formulario, Estudiante estudiante)
    {
        estudiante.Nombres = formulario.Nombres.Trim();
        estudiante.ApellidoPaterno = formulario.ApellidoPaterno.Trim();
        estudiante.ApellidoMaterno = formulario.ApellidoMaterno?.Trim();
        estudiante.FechaNacimiento = formulario.FechaNacimiento;
        estudiante.DireccionPais = formulario.DireccionPais.Trim();
        estudiante.DireccionCiudad = formulario.DireccionCiudad.Trim();
        estudiante.DireccionCalle = formulario.DireccionCalle.Trim();
        estudiante.SedeId = formulario.SedeId;
    }

    private static List<string> SepararLineas(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return new List<string>();
        }

        return texto
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(linea => linea.Trim())
            .Where(linea => linea.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task ValidarContactosAsync(
        int estudianteId, List<string> correos, List<string> telefonos)
    {
        if (correos.Count == 0)
        {
            ModelState.AddModelError(nameof(EstudianteFormViewModel.Correos),
                "Debe registrar al menos un correo.");
        }

        var validadorDeCorreo = new EmailAddressAttribute();

        foreach (var correo in correos)
        {
            if (!validadorDeCorreo.IsValid(correo))
            {
                ModelState.AddModelError(nameof(EstudianteFormViewModel.Correos),
                    $"El correo {correo} no tiene un formato valido.");
            }
        }

        var correoRepetido = await _context.CorreosEstudiante
            .Where(c => c.EstudianteId != estudianteId)
            .Select(c => c.Direccion)
            .FirstOrDefaultAsync(direccion => correos.Contains(direccion));

        if (correoRepetido != null)
        {
            ModelState.AddModelError(nameof(EstudianteFormViewModel.Correos),
                $"El correo {correoRepetido} ya esta registrado por otro estudiante.");
        }

        foreach (var telefono in telefonos)
        {
            if (telefono.Length < 7 || telefono.Length > 20)
            {
                ModelState.AddModelError(nameof(EstudianteFormViewModel.Telefonos),
                    $"El telefono {telefono} debe tener entre 7 y 20 caracteres.");
            }
        }
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
