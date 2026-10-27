namespace EduConecta.Models;

public class CursoPrerrequisito
{
    public int CursoId { get; set; }
    public int PrerrequisitoId { get; set; }

    public Curso Curso { get; set; } = null!;
    public Curso Prerrequisito { get; set; } = null!;
}
