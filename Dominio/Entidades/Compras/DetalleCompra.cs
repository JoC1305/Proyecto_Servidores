namespace Dominio.Entidades.Compras;

public class DetalleCompra
{
    public Guid Id { get; private set; }

    public Guid CompraId { get; private set; }

    public Guid ProductoId { get; private set; }

    public int Cantidad { get; private set; }

    public decimal PrecioCompra { get; private set; }
}