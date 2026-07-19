using Dominio.Comun;
using Dominio.Compartido;

namespace Dominio.Entidades.Inventario;

public class Producto : Entidad
{
    public CodigoBarras CodigoBarras { get; private set; } = null!;

    public Nombre Nombre { get; private set; } = null!;

    public Dinero PrecioCompra { get; private set; } = null!;

    public Dinero PrecioVenta { get; private set; } = null!;

    public Descripcion Descripcion { get; private set; } = null!;

    public Cantidad Stock { get; private set; } = null!;

    public Cantidad StockMinimo { get; private set; } = null!;

    public bool Activo { get; private set; }

    public DateTime FechaCreacion { get; private set; }

    public Guid IdCategoria { get; private set; }

    public Dinero Ganancia => PrecioVenta - PrecioCompra;

    private Producto()
    {
    }

    private Producto(
        CodigoBarras codigoBarras,
        Nombre nombre,
        Dinero precioCompra,
        Dinero precioVenta,
        Descripcion descripcion,
        Cantidad stock,
        Cantidad stockMinimo,
        Guid idCategoria,
        DateTime fechaCreacion
    )
    {
        if (precioCompra.Monto <= 0)
        {
            throw new ArgumentException("El precio de compra debe ser mayor que cero.");
        }

        if (precioVenta.Monto <= 0)
        {
            throw new ArgumentException("El precio de venta debe ser mayor que cero.");
        }

        if (precioVenta.Monto < precioCompra.Monto)
        {
            throw new ArgumentException("El precio de venta no puede ser menor que el precio de compra.");
        }

        if (idCategoria == Guid.Empty)
        {
            throw new ArgumentException("La categoria es obligatoria.", nameof(idCategoria));
        }

        CodigoBarras = codigoBarras;
        Nombre = nombre;
        PrecioCompra = precioCompra;
        PrecioVenta = precioVenta;
        Descripcion = descripcion;
        Stock = stock;
        StockMinimo = stockMinimo;
        Activo = true;
        IdCategoria = idCategoria;
        FechaCreacion = fechaCreacion;
    }

    public static Producto Crear(
        CodigoBarras codigoBarras,
        Nombre nombre,
        Dinero precioCompra,
        Dinero precioVenta,
        Guid idCategoria,
        Descripcion? descripcion = null,
        Cantidad? stock = null,
        Cantidad? stockMinimo = null,
        DateTime? fechaCreacion = null
    )
    {
        return new Producto(
            codigoBarras,
            nombre,
            precioCompra,
            precioVenta,
            descripcion ?? new Descripcion(string.Empty),
            stock ?? Cantidad.Cero,
            stockMinimo ?? Cantidad.Cero,
            idCategoria,
            fechaCreacion ?? DateTime.UtcNow
        );
    }

    public void AumentarStock(Cantidad cantidad)
    {
        Stock = new Cantidad(Stock.Valor + cantidad.Valor);
    }

    public void DisminuirStock(Cantidad cantidad)
    {
        if (Stock.Valor - cantidad.Valor < 0)
        {
            throw new InvalidOperationException("No hay stock suficiente para realizar la salida.");
        }

        Stock = new Cantidad(Stock.Valor - cantidad.Valor);
    }

    public void Activar() => Activo = true;

    public void Desactivar() => Activo = false;
}
