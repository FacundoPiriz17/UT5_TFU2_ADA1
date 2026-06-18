using UT5_TFU.Dominio;

namespace UT5_TFU.Services;

public sealed class NotificacionService : INotificacionService
{
    public string? CrearNotificacionPedidoListo(Pedido pedido, PuestoComida puestoComida)
    {
        if (pedido.Estado != EstadoPedido.ListoParaRetirar)
        {
            return null;
        }

        return $"Pedido de {puestoComida.Nombre} listo para retirar.";
    }
    
    
}
