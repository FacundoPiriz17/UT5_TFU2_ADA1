using UT5_TFU.Dominio;
using UT5_TFU.DTOs;

namespace UT5_TFU.Services;

public interface IPedidoService
{
    IReadOnlyList<ResumenPedidoResponse> ObtenerPedidos(int? clienteId = null);
    ResultadoService<DetallePedidoResponse> ObtenerPedido(int id);
    IReadOnlyList<ResumenPedidoResponse> ObtenerPedidosPorPuestoComida(int puestoComidaId, EstadoPedido? estado = null);
    ResultadoService<DetallePedidoResponse> CrearPedido(CrearPedidoRequest request);
    ResultadoService<ActualizarEstadoPedidoResponse> ActualizarEstado(int id, ActualizarEstadoPedidoRequest request);
    ResultadoService<DetallePedidoResponse> ConfirmarPago(int id, ConfirmarPagoRequest request);
}
