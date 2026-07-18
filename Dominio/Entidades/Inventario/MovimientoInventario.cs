namespace Dominio.Entidades.Inventario;

public class MovimientoInventario
{
    public Guid Id { get; private set; }

    public Guid ProductoId { get; private set; }

    public TipoMovimiento Tipo { get; private set; }

    public int Cantidad { get; private set; }

    public DateTime Fecha { get; private set; }

    public string Observacion { get; private set; }
}