using UT5_TFU.Dominio;

namespace UT5_TFU.Services;

public interface IPagoService
{
    Pago IniciarPago(MetodoPago metodo, decimal monto, bool simularRechazo);
    Pago ConfirmarPago(MetodoPago metodo, decimal monto, bool simularRechazo);
}
