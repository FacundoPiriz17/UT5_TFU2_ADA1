using UT5_TFU.Dominio;

namespace UT5_TFU.Services;

public sealed class PagoService : IPagoService
{
    private readonly IReadOnlyDictionary<MetodoPago, IPagoStrategy> estrategias;

    public PagoService(IEnumerable<IPagoStrategy> estrategias)
    {
        this.estrategias = estrategias.ToDictionary(estrategia => estrategia.Metodo);
    }

    public Pago IniciarPago(MetodoPago metodo, decimal monto, bool simularRechazo)
    {
        return ObtenerEstrategia(metodo).Iniciar(monto, simularRechazo);
    }

    public Pago ConfirmarPago(MetodoPago metodo, decimal monto, bool simularRechazo)
    {
        return ObtenerEstrategia(metodo).Confirmar(monto, simularRechazo);
    }

    private IPagoStrategy ObtenerEstrategia(MetodoPago metodo)
    {
        if (estrategias.TryGetValue(metodo, out var estrategia))
        {
            return estrategia;
        }

        throw new InvalidOperationException($"No existe una estrategia de pago para el metodo {metodo}.");
    }
}
