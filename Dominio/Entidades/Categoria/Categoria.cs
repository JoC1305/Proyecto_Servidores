using Dominio.Comun;
using Dominio.Compartido;
using Dominio.Entidades.Inventario;

namespace Dominio.Entidades.Categoria;

public class Categoria : Entidad
{
    public Nombre Nombre { get; private set; } = null!;

    public Descripcion Descripcion { get; private set; } = null!;

    private Categoria()
    {
    }

    private Categoria(Nombre nombre, Descripcion descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public static Categoria Crear(Nombre nombre, Descripcion descripcion)
    {
        return new Categoria(nombre, descripcion);
    }

    public void Actualizar(Nombre nombre, Descripcion descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }
}
