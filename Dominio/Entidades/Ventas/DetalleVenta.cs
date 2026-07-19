using Dominio.Compartido;
using Dominio.Entidades.Inventario;

namespace Dominio.Entidades.Ventas;

public class DetalleVenta
{
    public Guid Id { get; private set; }

    public Guid VentaId { get; private set; }

    public Guid ProductoId { get; private set; }

    public Cantidad Cantidad { get; private set; } = null!;

    public Dinero PrecioUnitario { get; private set; } = null!;

    public Dinero Subtotal => new(Cantidad.Valor * PrecioUnitario.Monto, PrecioUnitario.Moneda);

    private DetalleVenta(Guid ventaId, Guid productoId, Cantidad cantidad, Dinero precioUnitario)
    {
        if (ventaId == Guid.Empty)
        {
            throw new ArgumentException("La venta es obligatoria.", nameof(ventaId));
        }

        if (productoId == Guid.Empty)
        {
            throw new ArgumentException("El producto es obligatorio.", nameof(productoId));
        }

        Id = Guid.NewGuid();
        VentaId = ventaId;
        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }

    public static DetalleVenta Crear(Guid ventaId, Guid productoId, Cantidad cantidad, Dinero precioUnitario)
    {
        return new DetalleVenta(ventaId, productoId, cantidad, precioUnitario);
    }
}
