namespace Dominio.Entidades.Contabilidad;

public class CuentaContable
{
    public Guid Id { get; private set; }

    public string Codigo { get; private set; }

    public string Nombre { get; private set; }

    public TipoCuenta Tipo { get; private set; }
}