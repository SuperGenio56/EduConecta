namespace EduConecta.Helpers;

public static class OpcionesPaginacion
{
    public static readonly int[] Tamanos = { 5, 10, 25, 50 };

    public const int TamanoPredeterminado = 10;

    public static int Normalizar(int? tamanoPagina)
    {
        if (tamanoPagina.HasValue && Tamanos.Contains(tamanoPagina.Value))
        {
            return tamanoPagina.Value;
        }

        return TamanoPredeterminado;
    }
}
