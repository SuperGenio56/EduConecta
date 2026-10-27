namespace EduConecta.Models;

public class CursoDocente
{
    public int CursoId { get; set; }
    public int DocenteId { get; set; }

    public Curso Curso { get; set; } = null!;
    public Docente Docente { get; set; } = null!;
}
