using UT5_TFU.Dominio;

namespace UT5_TFU.Repositories;

public interface IFoodTrackRepository
{
    IReadOnlyList<PuestoComida> ObtenerPuestosComida();
    PuestoComida? ObtenerPuestoComida(int id);
    IReadOnlyList<Producto> ObtenerProductosPorPuestoComida(int puestoComidaId);
    Producto? ObtenerProducto(int puestoComidaId, int productoId);
    IReadOnlyList<Pedido> ObtenerPedidos();
    Pedido? ObtenerPedido(int id);
    Pedido AgregarPedido(Pedido pedido);
    void ActualizarPedido(Pedido pedido);
}
