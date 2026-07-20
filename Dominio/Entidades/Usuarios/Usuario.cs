using Dominio.Comun;
using Dominio.Compartido;

namespace Dominio.Entidades.Usuarios;

public class Usuario : Entidad
{
    public Nombre Nombre { get; private set; } = null!;

    public CorreoElectronico Email { get; private set; } = null!;

    public string HashContrasena { get; private set; } = null!;

    public RolUsuario Rol { get; private set; }

    private Usuario()
    {
    }

    private Usuario(Nombre nombre, CorreoElectronico email, string hashContrasena, RolUsuario rol)
    {
        if (string.IsNullOrWhiteSpace(hashContrasena))
        {
            throw new ArgumentException("La contrasena no puede estar vacia.", nameof(hashContrasena));
        }

        if (!Enum.IsDefined(typeof(RolUsuario), rol))
        {
            throw new ArgumentException("El rol proporcionado no es valido.", nameof(rol));
        }

        Nombre = nombre;
        Email = email;
        HashContrasena = hashContrasena;
        Rol = rol;
    }

    public static Usuario Crear(Nombre nombre, CorreoElectronico email, string hashContrasena, RolUsuario rol)
    {
        return new Usuario(nombre, email, hashContrasena, rol);
    }

    public void CambiarRol(RolUsuario rol)
    {
        if (!Enum.IsDefined(typeof(RolUsuario), rol))
        {
            throw new ArgumentException("El rol proporcionado no es valido.", nameof(rol));
        }

        Rol = rol;
    }

    public void CambiarContrasena(string hashContrasena)
    {
        if (string.IsNullOrWhiteSpace(hashContrasena))
        {
            throw new ArgumentException("La contrasena no puede estar vacia.", nameof(hashContrasena));
        }

        HashContrasena = hashContrasena;
    }
}
