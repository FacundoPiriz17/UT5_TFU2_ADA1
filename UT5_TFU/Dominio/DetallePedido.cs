namespace UT5_TFU.Dominio;

public sealed class DetallePedido
{
    public int ProductoId { get; init; }
    public required string NombreProducto { get; init; }
    public int Cantidad { get; init; }
    public decimal PrecioUnitario { get; init; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}
