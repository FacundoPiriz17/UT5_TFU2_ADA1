using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UT5_TFU.Dominio;
using UT5_TFU.DTOs;
using UT5_TFU.Services;

namespace UT5_TFU.Controllers;

[ApiController]
[Route("api/pedidos")]
[Tags("Pedidos")]
public sealed class PedidosController : ControllerBase
{
    private readonly IPedidoService pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        this.pedidoService = pedidoService;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Registra un pedido nuevo.",
        Description = "UC3 - Cliente. Crea un pedido con los productos seleccionados. El carrito se arma en el frontend y se envia como items del pedido."
    )]
    public ActionResult<DetallePedidoResponse> CrearPedido(CrearPedidoRequest request)
    {
        var resultado = pedidoService.CrearPedido(request);
        return resultado.Estado switch
        {
            EstadoResultadoService.Exito => CreatedAtAction(nameof(ObtenerPedido), new { id = resultado.Valor!.Id }, resultado.Valor),
            EstadoResultadoService.NoEncontrado => NotFound(new ErrorResponse(resultado.Error!)),
            _ => BadRequest(new ErrorResponse(resultado.Error!))
        };
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Consulta el detalle y estado de un pedido.",
        Description = "UC5 - Cliente. Devuelve el detalle del pedido, su pago, su estado actual y el progreso para seguimiento."
    )]
    public ActionResult<DetallePedidoResponse> ObtenerPedido(int id)
    {
        var resultado = pedidoService.ObtenerPedido(id);
        return ConvertirResultado(resultado);
    }

    [HttpGet("/api/clientes/{clienteId:int}/pedidos")]
    [SwaggerOperation(
        Summary = "Consulta los pedidos de un cliente.",
        Description = "UC5 - Cliente. Devuelve los pedidos realizados por el cliente para alimentar la pantalla de Pedidos Realizados."
    )]
    public ActionResult<IReadOnlyList<ResumenPedidoResponse>> ObtenerPedidosDelCliente(int clienteId)
    {
        return Ok(pedidoService.ObtenerPedidos(clienteId));
    }

    [HttpPost("{id:int}/preparar")]
    [SwaggerOperation(
        Summary = "Pasa un pedido a EnPreparacion.",
        Description = "UC8 - Cocinero. Marca el pedido como en preparacion."
    )]
    public ActionResult<ActualizarEstadoPedidoResponse> PrepararPedido(int id)
    {
        return CambiarEstado(id, EstadoPedido.EnPreparacion);
    }

    [HttpPost("{id:int}/listo")]
    [SwaggerOperation(
        Summary = "Pasa un pedido a ListoParaRetirar.",
        Description = "UC8/UC6 - Cocinero/Sistema. Marca el pedido como listo y dispara la notificacion simulada al cliente."
    )]
    public ActionResult<ActualizarEstadoPedidoResponse> MarcarPedidoListo(int id)
    {
        return CambiarEstado(id, EstadoPedido.ListoParaRetirar);
    }

    [HttpPost("{id:int}/entregar")]
    [SwaggerOperation(
        Summary = "Pasa un pedido a Entregado.",
        Description = "UC8 - Cajero. Marca el pedido como entregado al cliente."
    )]
    public ActionResult<ActualizarEstadoPedidoResponse> EntregarPedido(int id)
    {
        return CambiarEstado(id, EstadoPedido.Entregado);
    }

    [HttpPost("{id:int}/pago/confirmar")]
    [SwaggerOperation(
        Summary = "Confirma el cobro de un pedido.",
        Description = "UC4 - Cajero. Se usa especialmente para confirmar el cobro de pedidos en efectivo."
    )]
    public ActionResult<DetallePedidoResponse> ConfirmarPago(int id, ConfirmarPagoRequest request)
    {
        var resultado = pedidoService.ConfirmarPago(id, request);
        return ConvertirResultado(resultado);
    }
    
    [HttpPost("{id:int}/pago/realizar")]
    [SwaggerOperation(
        Summary = "Realiza el pago de un pedido",
        Description = "UC5 - Cliente. Se usa para realizar el pago del pedido."
    )]
    public ActionResult<DetallePedidoResponse> RealizarPago(int id)
    {
        var resultado = pedidoService.RealizarPago(id);
        return ConvertirResultado(resultado);
    }
    
    [HttpGet("{id:int}/pago/estado")]
    [SwaggerOperation(
        Summary = "Muestra el estado del pago.",
        Description = "UC4/UC5 - Cajero/Cliente. Devuelve el estado actual del pago del pedido."
    )]
    public ActionResult<EstadoPago> ObtenerEstadoPago(int id)
    {
        var resultado = pedidoService.ObtenerEstadoPago(id);
        return ConvertirResultado(resultado);
    }
    
    [HttpGet("/api/clientes/{clienteId:int}/notificaciones")]
    [SwaggerOperation(
        Summary = "Lista las notificaciones del cliente.",
        Description = "UC5 - Cliente. Devuelve avisos de pedidos listos para retirar."
    )]
    public ActionResult<IReadOnlyList<NotificacionResponse>> ObtenerNotificaciones(int clienteId)
    {
        return Ok(pedidoService.ObtenerNotificaciones(clienteId));
    }

    private ActionResult<ActualizarEstadoPedidoResponse> CambiarEstado(int id, EstadoPedido estado)
    {
        var request = new ActualizarEstadoPedidoRequest { Estado = estado };
        var resultado = pedidoService.ActualizarEstado(id, request);
        return ConvertirResultado(resultado);
    }

    private ActionResult<T> ConvertirResultado<T>(ResultadoService<T> resultado)
    {
        return resultado.Estado switch
        {
            EstadoResultadoService.Exito => Ok(resultado.Valor),
            EstadoResultadoService.NoEncontrado => NotFound(new ErrorResponse(resultado.Error!)),
            _ => BadRequest(new ErrorResponse(resultado.Error!))
        };
    }
}
