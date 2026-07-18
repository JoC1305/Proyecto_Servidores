namespace Dominio.Entidades.Proveedores;

public class Proveedor
{
    public Guid Id { get; private set; }

    public string Nombre { get; private set; }

    public string Telefono { get; private set; }

    public string Correo { get; private set; }
}