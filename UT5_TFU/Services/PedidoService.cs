using UT5_TFU.Dominio;
using UT5_TFU.DTOs;
using UT5_TFU.Repositories;

namespace UT5_TFU.Services;

public sealed class PedidoService : IPedidoService
{
    private static readonly EstadoPedido[] OrdenProgreso =
    {
        EstadoPedido.Pendiente,
        EstadoPedido.EnPreparacion,
        EstadoPedido.ListoParaRetirar,
        EstadoPedido.Entregado
    };

    private readonly IFoodTruckRepository repository;
    private readonly IPagoService pagoService;
    private readonly IEnumerable<IPedidoEstadoObserver> estadoObservers;

    public PedidoService(
        IFoodTruckRepository repository,
        IPagoService pagoService,
        IEnumerable<IPedidoEstadoObserver> estadoObservers)
    {
        this.repository = repository;
        this.pagoService = pagoService;
        this.estadoObservers = estadoObservers;
    }

    public IReadOnlyList<ResumenPedidoResponse> ObtenerPedidos(int? clienteId = null)
    {
        var pedidos = clienteId is null
            ? repository.ObtenerPedidos()
            : repository.ObtenerPedidosPorCliente(clienteId.Value);

        return pedidos
            .Select(ToResumenResponse)
            .ToList();
    }

    public IReadOnlyList<ResumenPedidoResponse> ObtenerPedidosPorPuestoComida(int puestoComidaId, EstadoPedido? estado = null)
    {
        return repository.ObtenerPedidosPorPuestoComida(puestoComidaId, estado)
            .Select(ToResumenResponse)
            .ToList();
    }

    public ResultadoService<DetallePedidoResponse> ObtenerPedido(int id)
    {
        var pedido = repository.ObtenerPedido(id);
        if (pedido is null)
        {
            return ResultadoService<DetallePedidoResponse>.NoEncontrado("No se encontro el pedido solicitado.");
        }

        return ResultadoService<DetallePedidoResponse>.Exito(ToDetalleResponse(pedido));
    }

    public ResultadoService<DetallePedidoResponse> CrearPedido(CrearPedidoRequest request)
    {
        if (request.ClienteId <= 0)
        {
            return ResultadoService<DetallePedidoResponse>.ErrorValidacion("El clienteId debe ser mayor a cero.");
        }

        if (string.IsNullOrWhiteSpace(request.NombreCliente))
        {
            return ResultadoService<DetallePedidoResponse>.ErrorValidacion("El nombre del cliente es obligatorio.");
        }

        var puestoComida = repository.ObtenerPuestoComida(request.PuestoComidaId);
        if (puestoComida is null)
        {
            return ResultadoService<DetallePedidoResponse>.NoEncontrado("No se encontro el puesto de comida solicitado.");
        }

        if (request.Items.Count == 0)
        {
            return ResultadoService<DetallePedidoResponse>.ErrorValidacion("El pedido debe tener al menos un producto.");
        }

        var detalles = new List<DetallePedido>();
        foreach (var itemRequest in request.Items)
        {
            if (itemRequest.Cantidad <= 0)
            {
                return ResultadoService<DetallePedidoResponse>.ErrorValidacion("La cantidad de cada producto debe ser mayor a cero.");
            }

            var producto = repository.ObtenerProducto(request.PuestoComidaId, itemRequest.ProductoId);
            if (producto is null)
            {
                return ResultadoService<DetallePedidoResponse>.NoEncontrado("No se encontro uno de los productos solicitados.");
            }

            if (producto.Stock < itemRequest.Cantidad)
            {
                return ResultadoService<DetallePedidoResponse>.ErrorValidacion($"No hay stock suficiente para {producto.Nombre}.");
            }

            detalles.Add(new DetallePedido
            {
                ProductoId = producto.Id,
                NombreProducto = producto.Nombre,
                Cantidad = itemRequest.Cantidad,
                PrecioUnitario = producto.Precio
            });
        }

        var montoTotal = detalles.Sum(detalle => detalle.Subtotal);
        var pago = pagoService.IniciarPago(
            request.MetodoPago,
            montoTotal,
            request.SimularRechazoPago);

        if (pago.Estado == EstadoPago.Rechazado)
        {
            return ResultadoService<DetallePedidoResponse>.ErrorValidacion("El pago fue rechazado y el pedido no quedo registrado.");
        }

        var pedido = new Pedido
        {
            ClienteId = request.ClienteId,
            NombreCliente = request.NombreCliente.Trim(),
            PuestoComidaId = request.PuestoComidaId,
            Pago = pago,
            Estado = EstadoPedido.EnPreparacion,
            FechaCreacion = DateTimeOffset.UtcNow
        };

        pedido.Detalles.AddRange(detalles);

        foreach (var detalle in detalles)
        {
            var producto = repository.ObtenerProducto(request.PuestoComidaId, detalle.ProductoId);
            if (producto is not null)
            {
                producto.Stock -= detalle.Cantidad;
            }
        }

        var pedidoGuardado = repository.AgregarPedido(pedido);
        return ResultadoService<DetallePedidoResponse>.Exito(ToDetalleResponse(pedidoGuardado));
    }

    public ResultadoService<ActualizarEstadoPedidoResponse> ActualizarEstado(int id, ActualizarEstadoPedidoRequest request)
    {
        var pedido = repository.ObtenerPedido(id);
        if (pedido is null)
        {
            return ResultadoService<ActualizarEstadoPedidoResponse>.NoEncontrado("No se encontro el pedido solicitado.");
        }

        var puestoComida = repository.ObtenerPuestoComida(pedido.PuestoComidaId);
        if (puestoComida is null)
        {
            return ResultadoService<ActualizarEstadoPedidoResponse>.ErrorValidacion("El pedido no tiene un puesto de comida valido.");
        }

        var estadoAnterior = pedido.Estado;
        var notificacionAnterior = pedido.UltimaNotificacion;

        pedido.Estado = request.Estado;
        pedido.FechaEntrega = request.Estado == EstadoPedido.Entregado
            ? DateTimeOffset.UtcNow
            : pedido.FechaEntrega;

        var evento = new PedidoEstadoCambiado(
            pedido,
            puestoComida,
            estadoAnterior,
            request.Estado);

        foreach (var observer in estadoObservers)
        {
            observer.AlCambiarEstado(evento);
        }

        repository.ActualizarPedido(pedido);

        var notificacionGenerada = pedido.UltimaNotificacion != notificacionAnterior
            ? pedido.UltimaNotificacion
            : null;

        var respuesta = new ActualizarEstadoPedidoResponse(ToDetalleResponse(pedido), notificacionGenerada);
        return ResultadoService<ActualizarEstadoPedidoResponse>.Exito(respuesta);
    }

    public ResultadoService<DetallePedidoResponse> ConfirmarPago(int id, ConfirmarPagoRequest request)
    {
        var pedido = repository.ObtenerPedido(id);
        if (pedido is null)
        {
            return ResultadoService<DetallePedidoResponse>.NoEncontrado("No se encontro el pedido solicitado.");
        }

        if (pedido.Pago.Estado == EstadoPago.Pagado)
        {
            return ResultadoService<DetallePedidoResponse>.Exito(ToDetalleResponse(pedido));
        }

        var metodo = request.MetodoPago ?? pedido.Pago.Metodo;
        var pago = pagoService.ConfirmarPago(
            metodo,
            pedido.MontoTotal,
            request.SimularRechazoPago);

        if (pago.Estado == EstadoPago.Rechazado)
        {
            pedido.Pago = pago;
            repository.ActualizarPedido(pedido);
            return ResultadoService<DetallePedidoResponse>.ErrorValidacion("El pago fue rechazado.");
        }

        pedido.Pago = pago;
        repository.ActualizarPedido(pedido);
        return ResultadoService<DetallePedidoResponse>.Exito(ToDetalleResponse(pedido));
    }
    public ResultadoService<EstadoPago> ObtenerEstadoPago(int id)
    {
        var pedido = repository.ObtenerPedido(id);

        if (pedido is null)
        {
            return ResultadoService<EstadoPago>.NoEncontrado("No se encontro el pedido solicitado.");
        }

        return ResultadoService<EstadoPago>.Exito(pedido.Pago.Estado);
    }
    private ResumenPedidoResponse ToResumenResponse(Pedido pedido)
    {
        var puestoComida = repository.ObtenerPuestoComida(pedido.PuestoComidaId);
        var nombrePuestoComida = puestoComida?.Nombre ?? "Puesto de comida desconocido";

        return new ResumenPedidoResponse(
            pedido.Id,
            pedido.ClienteId,
            pedido.NombreCliente,
            pedido.PuestoComidaId,
            nombrePuestoComida,
            pedido.Pago.Estado,
            pedido.Estado,
            pedido.MontoTotal,
            pedido.FechaCreacion,
            pedido.FechaEntrega,
            pedido.UltimaNotificacion);
    }

    private DetallePedidoResponse ToDetalleResponse(Pedido pedido)
    {
        var puestoComida = repository.ObtenerPuestoComida(pedido.PuestoComidaId);
        var nombrePuestoComida = puestoComida?.Nombre ?? "Puesto de comida desconocido";

        var items = pedido.Detalles
            .Select(detalle => new ItemPedidoResponse(
                detalle.ProductoId,
                detalle.NombreProducto,
                detalle.Cantidad,
                detalle.PrecioUnitario,
                detalle.Subtotal))
            .ToList();

        var pago = new PagoResponse(
            pedido.Pago.Metodo,
            pedido.Pago.Estado,
            pedido.Pago.Monto,
            pedido.Pago.FechaProcesado);

        return new DetallePedidoResponse(
            pedido.Id,
            pedido.ClienteId,
            pedido.NombreCliente,
            pedido.PuestoComidaId,
            nombrePuestoComida,
            items,
            pago,
            pedido.Estado,
            pedido.MontoTotal,
            pedido.FechaCreacion,
            pedido.FechaEntrega,
            ConstruirProgreso(pedido.Estado),
            pedido.UltimaNotificacion);
    }

    private static IReadOnlyList<PasoProgresoPedidoResponse> ConstruirProgreso(EstadoPedido estadoActual)
    {
        if (estadoActual == EstadoPedido.Cancelado)
        {
            return new[]
            {
                new PasoProgresoPedidoResponse(EstadoPedido.Cancelado, "Cancelado", true, true)
            };
        }

        var indiceActual = Array.IndexOf(OrdenProgreso, estadoActual);

        return OrdenProgreso
            .Select((estado, indice) => new PasoProgresoPedidoResponse(
                estado,
                ObtenerEtiquetaProgreso(estado),
                indiceActual >= 0 && indice <= indiceActual,
                estado == estadoActual))
            .ToList();
    }

    private static string ObtenerEtiquetaProgreso(EstadoPedido estado)
    {
        return estado switch
        {
            EstadoPedido.Pendiente => "Pedido pendiente",
            EstadoPedido.EnPreparacion => "En preparacion",
            EstadoPedido.ListoParaRetirar => "Listo para retirar",
            EstadoPedido.Entregado => "Entregado",
            EstadoPedido.Cancelado => "Cancelado",
            _ => estado.ToString()
        };
    }
}
