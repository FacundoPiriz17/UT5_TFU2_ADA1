namespace UT5_TFU.Dominio;

public sealed record PedidoEstadoCambiado(
    Pedido Pedido,
    PuestoComida PuestoComida,
    EstadoPedido EstadoAnterior,
    EstadoPedido EstadoNuevo);
