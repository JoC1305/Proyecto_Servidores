using Dominio.Comun;
using Dominio.Compartido;
using Dominio.Entidades.Inventario;
using Dominio.Eventos;

namespace Dominio.Entidades.Ventas;

public class Venta : Entidad
{
    public DateTime Fecha { get; private set; }

    public Dinero Total { get; private set; } = null!;

    public MetodoPago MetodoPago { get; private set; }

    public Guid UsuarioId { get; private set; }

    public ICollection<DetalleVenta> Detalles { get; private set; } = new List<DetalleVenta>();

    private Venta()
    {
    }

    private Venta(Guid usuarioId, MetodoPago metodoPago, DateTime fecha)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("El usuario es obligatorio.", nameof(usuarioId));
        }

        if (!Enum.IsDefined(typeof(MetodoPago), metodoPago))
        {
            throw new ArgumentException("El metodo de pago no es valido.", nameof(metodoPago));
        }

        Fecha = fecha == default ? DateTime.UtcNow : fecha;
        Total = Dinero.Cero();
        MetodoPago = metodoPago;
        UsuarioId = usuarioId;
        Detalles = new List<DetalleVenta>();
    }

    public static Venta Crear(Guid usuarioId, MetodoPago metodoPago, DateTime? fecha = null)
    {
        return new Venta(usuarioId, metodoPago, fecha ?? DateTime.UtcNow);
    }

    public void AgregarDetalle(Guid productoId, Cantidad cantidad, Dinero precioUnitario)
    {
        var detalle = DetalleVenta.Crear(Id, productoId, cantidad, precioUnitario);
        Detalles.Add(detalle);
        Total = Total + detalle.Subtotal;
    }

    public void RegistrarEventoVentaRegistrada()
    {
        if (Detalles.Count == 0)
        {
            throw new InvalidOperationException("La venta debe tener al menos un detalle.");
        }

        RegistrarEventoDominio(new VentaRegistrada(Id, UsuarioId, Total.Monto));
    }
}
