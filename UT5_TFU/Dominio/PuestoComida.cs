namespace UT5_TFU.Dominio;

public sealed class PuestoComida
{
    public int Id { get; init; }
    public required string Nombre { get; init; }
    public required string Descripcion { get; init; }
    public string? LogoUrl { get; init; }
}
