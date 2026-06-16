using UT5_TFU.Dominio;

namespace UT5_TFU.DTOs;

public sealed record PuestoComidaResponse(
    int Id,
    string Nombre,
    string Descripcion,
    string? LogoUrl);

public sealed record ProductoResponse(
    int Id,
    int PuestoComidaId,
    string Nombre,
    string Descripcion,
    string? ImagenUrl,
    decimal Precio,
    int Stock,
    bool Disponible);

public sealed record MenuPuestoComidaResponse(
    PuestoComidaResponse PuestoComida,
    IReadOnlyList<ProductoResponse> Productos);

public static class PuestoComidaDtoMapper
{
    public static PuestoComidaResponse ToResponse(PuestoComida puestoComida)
    {
        return new PuestoComidaResponse(
            puestoComida.Id,
            puestoComida.Nombre,
            puestoComida.Descripcion,
            puestoComida.LogoUrl);
    }

    public static ProductoResponse ToResponse(Producto producto)
    {
        return new ProductoResponse(
            producto.Id,
            producto.PuestoComidaId,
            producto.Nombre,
            producto.Descripcion,
            producto.ImagenUrl,
            producto.Precio,
            producto.Stock,
            producto.Disponible);
    }
}
