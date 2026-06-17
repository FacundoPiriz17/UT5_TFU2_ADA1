using UT5_TFU.Dominio;

namespace UT5_TFU.Services;

public sealed class PedidoListoNotificacionObserver : IPedidoEstadoObserver
{
    private readonly INotificacionService notificacionService;

    public PedidoListoNotificacionObserver(INotificacionService notificacionService)
    {
        this.notificacionService = notificacionService;
    }

    public void AlCambiarEstado(PedidoEstadoCambiado evento)
    {
        if (evento.EstadoNuevo != EstadoPedido.ListoParaRetirar ||
            evento.EstadoAnterior == EstadoPedido.ListoParaRetirar)
        {
            return;
        }

        var notificacion = notificacionService.CrearNotificacionPedidoListo(evento.Pedido, evento.PuestoComida);
        if (notificacion is not null)
        {
            evento.Pedido.UltimaNotificacion = notificacion;
        }
    }
}
