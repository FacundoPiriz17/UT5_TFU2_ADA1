using UT5_TFU.Dominio;

namespace UT5_TFU.Services;

public sealed class PagoStrategyEfectivo : IPagoStrategy
{
    public MetodoPago Metodo => MetodoPago.Efectivo;

    public Pago Iniciar(decimal monto, bool simularRechazo)
    {
        return new Pago
        {
            Metodo = Metodo,
            Estado = simularRechazo ? EstadoPago.Rechazado : EstadoPago.Pendiente,
            Monto = monto
        };
    }

    public Pago Confirmar(decimal monto, bool simularRechazo)
    {
        return new Pago
        {
            Metodo = Metodo,
            Estado = simularRechazo ? EstadoPago.Rechazado : EstadoPago.Pagado,
            Monto = monto
        };
    }
}

public sealed class PagoStrategyTarjeta : IPagoStrategy
{
    public MetodoPago Metodo => MetodoPago.Tarjeta;

    public Pago Iniciar(decimal monto, bool simularRechazo)
    {
        return ConstruirPago(monto, simularRechazo);
    }

    public Pago Confirmar(decimal monto, bool simularRechazo)
    {
        return ConstruirPago(monto, simularRechazo);
    }

    private Pago ConstruirPago(decimal monto, bool simularRechazo)
    {
        return new Pago
        {
            Metodo = Metodo,
            Estado = simularRechazo ? EstadoPago.Rechazado : EstadoPago.Pagado,
            Monto = monto
        };
    }
}

public sealed class PagoStrategyMercadoPagoStrategy : IPagoStrategy
{
    public MetodoPago Metodo => MetodoPago.MercadoPago;

    public Pago Iniciar(decimal monto, bool simularRechazo)
    {
        return ConstruirPago(monto, simularRechazo);
    }

    public Pago Confirmar(decimal monto, bool simularRechazo)
    {
        return ConstruirPago(monto, simularRechazo);
    }

    private Pago ConstruirPago(decimal monto, bool simularRechazo)
    {
        return new Pago
        {
            Metodo = Metodo,
            Estado = simularRechazo ? EstadoPago.Rechazado : EstadoPago.Pagado,
            Monto = monto
        };
    }
}
