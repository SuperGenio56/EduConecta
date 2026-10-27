using System.Diagnostics;
using EduConecta.Data;
using EduConecta.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduConecta.Controllers;

public class HomeController : Controller
{
    private readonly EduConectaContext _context;

    public HomeController(EduConectaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var resumen = new ResumenInicioViewModel
        {
            TotalSedes = await _context.Sedes.CountAsync(),
            TotalEstudiantes = await _context.Estudiantes.CountAsync(),
            TotalBecas = await _context.Becas.CountAsync(),
            TotalDocentes = await _context.Docentes.CountAsync(),
            TotalCursos = await _context.Cursos.CountAsync()
        };

        return View(resumen);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
