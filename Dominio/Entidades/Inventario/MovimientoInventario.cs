using Dominio.Comun;

namespace Dominio.Entidades.Inventario;

public class MovimientoInventario : Entidad
{
    public Guid ProductoId { get; private set; }

    public TipoMovimiento Tipo { get; private set; }

    public Cantidad Cantidad { get; private set; } = null!;

    public DateTime Fecha { get; private set; }

    public string Observacion { get; private set; } = string.Empty;

    private MovimientoInventario()
    {
    }

    private MovimientoInventario(Guid productoId, TipoMovimiento tipo, Cantidad cantidad, DateTime fecha, string? observacion)
    {
        if (productoId == Guid.Empty)
        {
            throw new ArgumentException("El producto es obligatorio.", nameof(productoId));
        }

        if (!Enum.IsDefined(typeof(TipoMovimiento), tipo))
        {
            throw new ArgumentException("El tipo de movimiento no es valido.", nameof(tipo));
        }

        ProductoId = productoId;
        Tipo = tipo;
        Cantidad = cantidad;
        Fecha = fecha == default ? DateTime.UtcNow : fecha;
        Observacion = observacion?.Trim() ?? string.Empty;
    }

    public static MovimientoInventario CrearEntrada(Guid productoId, Cantidad cantidad, string? observacion = null, DateTime? fecha = null)
    {
        return new MovimientoInventario(productoId, TipoMovimiento.Entrada, cantidad, fecha ?? DateTime.UtcNow, observacion);
    }

    public static MovimientoInventario CrearSalida(Guid productoId, Cantidad cantidad, string? observacion = null, DateTime? fecha = null)
    {
        return new MovimientoInventario(productoId, TipoMovimiento.Salida, cantidad, fecha ?? DateTime.UtcNow, observacion);
    }
}
