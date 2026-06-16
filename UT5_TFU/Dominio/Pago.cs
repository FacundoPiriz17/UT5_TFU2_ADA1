namespace UT5_TFU.Dominio;

public sealed class Pago
{
    public MetodoPago Metodo { get; init; }
    public EstadoPago Estado { get; set; }
    public decimal Monto { get; init; }
    public DateTimeOffset FechaProcesado { get; init; } = DateTimeOffset.UtcNow;
}
