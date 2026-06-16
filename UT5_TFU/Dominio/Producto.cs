namespace UT5_TFU.Dominio;

public sealed class Producto
{
    public int Id { get; init; }
    public int PuestoComidaId { get; init; }
    public required string Nombre { get; init; }
    public required string Descripcion { get; init; }
    public string? ImagenUrl { get; init; }
    public decimal Precio { get; init; }
    public int Stock { get; set; }
    public bool Disponible => Stock > 0;
}
