using EduConecta.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConecta.Data;

public class EduConectaContext : DbContext
{
    public EduConectaContext(DbContextOptions<EduConectaContext> options)
        : base(options) { }

    public DbSet<Sede> Sedes => Set<Sede>();
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<CorreoEstudiante> CorreosEstudiante => Set<CorreoEstudiante>();
    public DbSet<TelefonoEstudiante> TelefonosEstudiante => Set<TelefonoEstudiante>();
    public DbSet<Beca> Becas => Set<Beca>();
    public DbSet<Docente> Docentes => Set<Docente>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<CursoDocente> CursosDocentes => Set<CursoDocente>();
    public DbSet<CursoPrerrequisito> CursosPrerrequisitos => Set<CursoPrerrequisito>();
    public DbSet<Modulo> Modulos => Set<Modulo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EduConectaContext).Assembly);
    }
}
