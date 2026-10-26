namespace EduConecta.Helpers;

public interface IListaPaginada
{
    int PaginaActual { get; }
    int TotalPaginas { get; }
    int TamanoPagina { get; }
    int TotalRegistros { get; }
    bool TienePaginaAnterior { get; }
    bool TienePaginaSiguiente { get; }
}
