namespace Dominio.Entidades.Contabilidad;

public class DetalleAsiento
{
    public Guid Id { get; private set; }

    public Guid AsientoContableId { get; private set; }

    public Guid CuentaContableId { get; private set; }

    public decimal Debe { get; private set; }

    public decimal Haber { get; private set; }
}