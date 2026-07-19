using Dominio.Comun;
using Dominio.Compartido;

namespace Dominio.Entidades.Proveedores;

public class Proveedor : Entidad
{
    public Nombre Nombre { get; private set; } = null!;

    public Telefono? Telefono { get; private set; }

    public CorreoElectronico? Correo { get; private set; }

    private Proveedor()
    {
    }

    private Proveedor(Nombre nombre, Telefono? telefono, CorreoElectronico? correo)
    {
        Nombre = nombre;
        Telefono = telefono;
        Correo = correo;
    }

    public static Proveedor Crear(Nombre nombre, Telefono? telefono = null, CorreoElectronico? correo = null)
    {
        return new Proveedor(nombre, telefono, correo);
    }

    public void ActualizarContacto(Telefono? telefono, CorreoElectronico? correo)
    {
        Telefono = telefono;
        Correo = correo;
    }
}
