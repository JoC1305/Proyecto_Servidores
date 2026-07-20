using Dominio.Comun;
using Dominio.Compartido;

namespace Dominio.Entidades.Clientes;

public class Cliente : Entidad
{
    public Cedula Cedula { get; private set; } = null!;

    public Nombre Nombre { get; private set; } = null!;

    public Telefono? Telefono { get; private set; }

    public CorreoElectronico? Correo { get; private set; }

    private Cliente()
    {
    }

    private Cliente(Cedula cedula, Nombre nombre, Telefono? telefono, CorreoElectronico? correo)
    {
        Cedula = cedula;
        Nombre = nombre;
        Telefono = telefono;
        Correo = correo;
    }

    public static Cliente Crear(Cedula cedula, Nombre nombre, Telefono? telefono = null, CorreoElectronico? correo = null)
    {
        return new Cliente(cedula, nombre, telefono, correo);
    }

    public void ActualizarContacto(Telefono? telefono, CorreoElectronico? correo)
    {
        Telefono = telefono;
        Correo = correo;
    }
}
