using UT5_TFU.DTOs;
using UT5_TFU.Repositories;

namespace UT5_TFU.Services;

public sealed class PuestoComidaService : IPuestoComidaService
{
    private readonly IFoodTruckRepository repository;

    public PuestoComidaService(IFoodTruckRepository repository)
    {
        this.repository = repository;
    }

    public IReadOnlyList<PuestoComidaResponse> ObtenerPuestosComida()
    {
        return repository.ObtenerPuestosComida()
            .Select(PuestoComidaDtoMapper.ToResponse)
            .ToList();
    }

    public ResultadoService<PuestoComidaResponse> ObtenerPuestoComida(int id)
    {
        var puestoComida = repository.ObtenerPuestoComida(id);
        if (puestoComida is null)
        {
            return ResultadoService<PuestoComidaResponse>.NoEncontrado("No se encontro el puesto de comida solicitado.");
        }

        return ResultadoService<PuestoComidaResponse>.Exito(PuestoComidaDtoMapper.ToResponse(puestoComida));
    }

    public ResultadoService<MenuPuestoComidaResponse> ObtenerMenu(int puestoComidaId)
    {
        var puestoComida = repository.ObtenerPuestoComida(puestoComidaId);
        if (puestoComida is null)
        {
            return ResultadoService<MenuPuestoComidaResponse>.NoEncontrado("No se encontro el puesto de comida solicitado.");
        }

        var productos = repository.ObtenerProductosPorPuestoComida(puestoComidaId)
            .Select(PuestoComidaDtoMapper.ToResponse)
            .ToList();

        var respuesta = new MenuPuestoComidaResponse(
            PuestoComidaDtoMapper.ToResponse(puestoComida),
            productos);

        return ResultadoService<MenuPuestoComidaResponse>.Exito(respuesta);
    }
}
