using UT5_TFU.Dominio;

namespace UT5_TFU.DTOs;

public sealed class CrearPedidoRequest
{
    public int ClienteId { get; init; }
    public string NombreCliente { get; init; } = string.Empty;
    public int PuestoComidaId { get; init; }
    public MetodoPago MetodoPago { get; init; }
    public List<ItemPedidoRequest> Items { get; init; } = new();
    public bool SimularRechazoPago { get; init; }
}

public sealed record NotificacionResponse(
    int PedidoId,
    string Mensaje);

public sealed class ItemPedidoRequest
{
    public int ProductoId { get; init; }
    public int Cantidad { get; init; }
}

public sealed class ActualizarEstadoPedidoRequest
{
    public EstadoPedido Estado { get; init; }
}

public sealed class ConfirmarPagoRequest
{
    public MetodoPago? MetodoPago { get; init; }
    public bool SimularRechazoPago { get; init; }
}

public sealed class RealizarPagoRequest
{
    public MetodoPago MetodoPago { get; init; }
    public bool SimularRechazoPago { get; init; }
}
public sealed record ResumenPedidoResponse(
    int Id,
    int ClienteId,
    string NombreCliente,
    int PuestoComidaId,
    string NombrePuestoComida,
    EstadoPago EstadoPago,
    EstadoPedido EstadoPedido,
    decimal MontoTotal,
    DateTimeOffset FechaCreacion,
    DateTimeOffset? FechaEntrega,
    string? Notificacion);

public sealed record DetallePedidoResponse(
    int Id,
    int ClienteId,
    string NombreCliente,
    int PuestoComidaId,
    string NombrePuestoComida,
    IReadOnlyList<ItemPedidoResponse> Items,
    PagoResponse Pago,
    EstadoPedido EstadoPedido,
    decimal MontoTotal,
    DateTimeOffset FechaCreacion,
    DateTimeOffset? FechaEntrega,
    IReadOnlyList<PasoProgresoPedidoResponse> Progreso,
    string? Notificacion);

public sealed record ItemPedidoResponse(
    int ProductoId,
    string NombreProducto,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);

public sealed record PagoResponse(
    MetodoPago Metodo,
    EstadoPago Estado,
    decimal Monto,
    DateTimeOffset FechaProcesado);

public sealed record PasoProgresoPedidoResponse(
    EstadoPedido Estado,
    string Etiqueta,
    bool Completado,
    bool Actual);

public sealed record ActualizarEstadoPedidoResponse(
    DetallePedidoResponse Pedido,
    string? Notificacion);

public sealed record ErrorResponse(string Mensaje);
