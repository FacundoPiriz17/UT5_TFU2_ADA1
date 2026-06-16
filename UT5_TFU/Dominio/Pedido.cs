namespace UT5_TFU.Dominio;

public sealed class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; init; }
    public required string NombreCliente { get; init; }
    public int PuestoComidaId { get; init; }
    public List<DetallePedido> Detalles { get; } = new();
    public required Pago Pago { get; set; }
    public EstadoPedido Estado { get; set; }
    public DateTimeOffset FechaCreacion { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? FechaEntrega { get; set; }
    public string? UltimaNotificacion { get; set; }

    public decimal MontoTotal => Detalles.Sum(detalle => detalle.Subtotal);
}
