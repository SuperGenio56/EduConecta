using Microsoft.EntityFrameworkCore;

namespace EduConecta.Helpers;

public class ListaPaginada<T> : List<T>, IListaPaginada
{
    public int PaginaActual { get; private set; }
    public int TotalPaginas { get; private set; }
    public int TamanoPagina { get; private set; }
    public int TotalRegistros { get; private set; }

    public bool TienePaginaAnterior => PaginaActual > 1;
    public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

    public ListaPaginada(List<T> elementos, int totalRegistros, int paginaActual, int tamanoPagina)
    {
        TotalRegistros = totalRegistros;
        TamanoPagina = tamanoPagina;
        TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanoPagina);
        PaginaActual = paginaActual;

        AddRange(elementos);
    }

    public static async Task<ListaPaginada<T>> CrearAsync(
        IQueryable<T> consulta, int paginaActual, int tamanoPagina)
    {
        var totalRegistros = await consulta.CountAsync();

        var elementos = await consulta
            .Skip((paginaActual - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return new ListaPaginada<T>(elementos, totalRegistros, paginaActual, tamanoPagina);
    }
}
