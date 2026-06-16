using UT5_TFU.Dominio;

namespace UT5_TFU.Services;

public interface IPedidoEstadoObserver
{
    void AlCambiarEstado(PedidoEstadoCambiado evento);
}
