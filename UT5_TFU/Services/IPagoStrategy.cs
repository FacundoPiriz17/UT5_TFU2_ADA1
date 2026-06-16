using UT5_TFU.Dominio;

namespace UT5_TFU.Services;

public interface IPagoStrategy
{
    MetodoPago Metodo { get; }
    Pago Iniciar(decimal monto, bool simularRechazo);
    Pago Confirmar(decimal monto, bool simularRechazo);
}
