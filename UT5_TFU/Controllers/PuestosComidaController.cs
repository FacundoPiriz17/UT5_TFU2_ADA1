using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UT5_TFU.Dominio;
using UT5_TFU.DTOs;
using UT5_TFU.Services;

namespace UT5_TFU.Controllers;

[ApiController]
[Route("api/foodtrucks")]
[Tags("Foodtrucks")]
public sealed class PuestosComidaController : ControllerBase
{
    private readonly IPuestoComidaService puestoComidaService;
    private readonly IPedidoService pedidoService;

    public PuestosComidaController(IPuestoComidaService puestoComidaService, IPedidoService pedidoService)
    {
        this.puestoComidaService = puestoComidaService;
        this.pedidoService = pedidoService;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Visualiza los food trucks disponibles.",
        Description = "UC1 - Cliente. Devuelve la lista de food trucks que el cliente puede elegir en la pantalla principal."
    )]
    public ActionResult<IReadOnlyList<PuestoComidaResponse>> ObtenerFoodtrucks()
    {
        return Ok(puestoComidaService.ObtenerPuestosComida());
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Visualiza el detalle de un food truck.",
        Description = "Endpoint opcional para cliente. Devuelve los datos basicos del food truck seleccionado."
    )]
    public ActionResult<PuestoComidaResponse> ObtenerFoodtruck(int id)
    {
        var resultado = puestoComidaService.ObtenerPuestoComida(id);
        return ConvertirResultado(resultado);
    }

    [HttpGet("{id:int}/menu")]
    [SwaggerOperation(
        Summary = "Visualiza el menu de un food truck.",
        Description = "UC2 - Cliente. Devuelve los productos disponibles del food truck seleccionado."
    )]
    public ActionResult<MenuPuestoComidaResponse> ObtenerMenu(int id)
    {
        var resultado = puestoComidaService.ObtenerMenu(id);
        return ConvertirResultado(resultado);
    }

    [HttpGet("{id:int}/pedidos")]
    [SwaggerOperation(
        Summary = "Visualiza pedidos confirmados de un food truck.",
        Description = "UC7 - Cocinero. Devuelve los pedidos asociados al food truck y permite filtrarlos por estado mediante query string."
    )]
    public ActionResult<IReadOnlyList<ResumenPedidoResponse>> ObtenerPedidosDelFoodtruck(
        int id,
        [FromQuery] EstadoPedido? estado)
    {
        var puesto = puestoComidaService.ObtenerPuestoComida(id);
        if (puesto.Estado == EstadoResultadoService.NoEncontrado)
        {
            return NotFound(new ErrorResponse(puesto.Error!));
        }

        return Ok(pedidoService.ObtenerPedidosPorPuestoComida(id, estado));
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
