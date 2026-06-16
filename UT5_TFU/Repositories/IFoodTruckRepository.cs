using UT5_TFU.Dominio;

namespace UT5_TFU.Repositories;

public interface IFoodTruckRepository
{
    IReadOnlyList<PuestoComida> ObtenerPuestosComida();
    PuestoComida? ObtenerPuestoComida(int id);
    IReadOnlyList<Producto> ObtenerProductosPorPuestoComida(int puestoComidaId);
    Producto? ObtenerProducto(int puestoComidaId, int productoId);
    IReadOnlyList<Pedido> ObtenerPedidos();
    IReadOnlyList<Pedido> ObtenerPedidosPorCliente(int clienteId);
    IReadOnlyList<Pedido> ObtenerPedidosPorPuestoComida(int puestoComidaId, EstadoPedido? estado = null);
    Pedido? ObtenerPedido(int id);
    Pedido AgregarPedido(Pedido pedido);
    void ActualizarPedido(Pedido pedido);
}
