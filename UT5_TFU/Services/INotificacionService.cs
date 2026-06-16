using UT5_TFU.Dominio;

namespace UT5_TFU.Services;

public interface INotificacionService
{
    string? CrearNotificacionPedidoListo(Pedido pedido, PuestoComida puestoComida);
}
