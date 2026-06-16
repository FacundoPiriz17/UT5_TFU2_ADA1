namespace UT5_TFU.Services;

public enum EstadoResultadoService
{
    Exito,
    NoEncontrado,
    ErrorValidacion
}

public sealed class ResultadoService<T>
{
    private ResultadoService(EstadoResultadoService estado, T? valor, string? error)
    {
        Estado = estado;
        Valor = valor;
        Error = error;
    }

    public EstadoResultadoService Estado { get; }
    public T? Valor { get; }
    public string? Error { get; }

    public static ResultadoService<T> Exito(T valor)
    {
        return new ResultadoService<T>(EstadoResultadoService.Exito, valor, null);
    }

    public static ResultadoService<T> NoEncontrado(string error)
    {
        return new ResultadoService<T>(EstadoResultadoService.NoEncontrado, default, error);
    }

    public static ResultadoService<T> ErrorValidacion(string error)
    {
        return new ResultadoService<T>(EstadoResultadoService.ErrorValidacion, default, error);
    }
}
