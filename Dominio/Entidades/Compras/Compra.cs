namespace Dominio.Entidades.Compras;

public class Compra
{
    public Guid Id { get; private set; }

    public Guid ProveedorId { get; private set; }

    public DateTime Fecha { get; private set; }

    public decimal Total { get; private set; }
}