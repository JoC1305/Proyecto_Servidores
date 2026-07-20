using Dominio.Compartido;
using Dominio.Entidades.Inventario;

namespace Dominio.Entidades.Compras;

public class DetalleCompra
{
    public Guid Id { get; private set; }

    public Guid CompraId { get; private set; }

    public Guid ProductoId { get; private set; }

    public Cantidad Cantidad { get; private set; } = null!;

    public Dinero PrecioCompra { get; private set; } = null!;

    public Dinero Subtotal => new(Cantidad.Valor * PrecioCompra.Monto, PrecioCompra.Moneda);

    private DetalleCompra(Guid compraId, Guid productoId, Cantidad cantidad, Dinero precioCompra)
    {
        if (compraId == Guid.Empty)
        {
            throw new ArgumentException("La compra es obligatoria.", nameof(compraId));
        }

        if (productoId == Guid.Empty)
        {
            throw new ArgumentException("El producto es obligatorio.", nameof(productoId));
        }

        Id = Guid.NewGuid();
        CompraId = compraId;
        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioCompra = precioCompra;
    }

    public static DetalleCompra Crear(Guid compraId, Guid productoId, Cantidad cantidad, Dinero precioCompra)
    {
        return new DetalleCompra(compraId, productoId, cantidad, precioCompra);
    }
}
