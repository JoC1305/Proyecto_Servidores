namespace Dominio.Entidades.Ventas;

public class Venta
{
    public Guid Id { get; private set; }

    public DateTime Fecha { get; private set; }

    public decimal Total { get; private set; }

    public bool? EsCredito { get; private set; }

    public MetodoPago MetodoPago { get; private set; }

    public Guid UsuarioId { get; private set; }

    public ICollection<DetalleVenta> Detalles { get; private set; }
}