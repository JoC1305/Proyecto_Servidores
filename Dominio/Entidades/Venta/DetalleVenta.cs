namespace Dominio.Entidades.Ventas;

public class DetalleVenta
{
    public Guid Id { get; private set; }

    public Guid VentaId { get; private set; }

    public Guid ProductoId { get; private set; }

    public int Cantidad { get; private set; }

    public decimal PrecioUnitario { get; private set; }

    public decimal Subtotal => Cantidad * PrecioUnitario;
}