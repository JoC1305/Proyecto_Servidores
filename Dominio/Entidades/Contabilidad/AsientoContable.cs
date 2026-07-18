namespace Dominio.Entidades.Contabilidad;

public class AsientoContable
{
    public Guid Id { get; private set; }

    public DateTime Fecha { get; private set; }

    public string Descripcion { get; private set; }

    public ICollection<DetalleAsiento> Detalles { get; private set; }
}