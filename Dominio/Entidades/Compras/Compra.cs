using Dominio.Comun;
using Dominio.Compartido;
using Dominio.Entidades.Inventario;

namespace Dominio.Entidades.Compras;

public class Compra : Entidad
{
    public Guid ProveedorId { get; private set; }

    public Guid PedidoId { get; private set; }

    public DateTime Fecha { get; private set; }

    public Dinero Total { get; private set; } = null!;

    public ICollection<DetalleCompra> Detalles { get; private set; } = new List<DetalleCompra>();

    private Compra()
    {
    }

    private Compra(Guid proveedorId, Guid pedidoId, DateTime fecha)
    {
        if (proveedorId == Guid.Empty)
        {
            throw new ArgumentException("El proveedor es obligatorio.", nameof(proveedorId));
        }

        if (pedidoId == Guid.Empty)
        {
            throw new ArgumentException("El pedido es obligatorio.", nameof(pedidoId));
        }

        ProveedorId = proveedorId;
        PedidoId = pedidoId;
        Fecha = fecha == default ? DateTime.UtcNow : fecha;
        Total = Dinero.Cero();
        Detalles = new List<DetalleCompra>();
    }

    public static Compra Crear(Guid proveedorId, Guid pedidoId, DateTime? fecha = null)
    {
        return new Compra(proveedorId, pedidoId, fecha ?? DateTime.UtcNow);
    }

    public void AgregarDetalle(Guid productoId, Cantidad cantidad, Dinero precioCompra)
    {
        var detalle = DetalleCompra.Crear(Id, productoId, cantidad, precioCompra);
        Detalles.Add(detalle);
        Total = Total + detalle.Subtotal;
    }
}
