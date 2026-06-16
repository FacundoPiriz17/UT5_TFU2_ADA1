using UT5_TFU.DTOs;

namespace UT5_TFU.Services;

public interface IPuestoComidaService
{
    IReadOnlyList<PuestoComidaResponse> ObtenerPuestosComida();
    ResultadoService<PuestoComidaResponse> ObtenerPuestoComida(int id);
    ResultadoService<MenuPuestoComidaResponse> ObtenerMenu(int puestoComidaId);
}
