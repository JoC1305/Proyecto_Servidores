using Aplicacion.Abstracciones.Eventos;
using Aplicacion.Abstracciones.Persistencia;
using Aplicacion.Comun.CQRS;
using Aplicacion.Comun.Resultados;
using Dominio.Compartido;
using Dominio.Entidades.Contabilidad;
using Dominio.Entidades.Inventario;
using Dominio.Entidades.Ventas;

namespace Aplicacion.Ventas.RegistrarVenta;

public sealed class RegistrarVentaCommandHandler : ICommandHandler<RegistrarVentaCommand, Guid>
{
    private readonly IDomainEventDispatcher domainEventDispatcher;
    private readonly IRepositorioAsientoContable repositorioAsientoContable;
    private readonly IRepositorioMovimientoInventario repositorioMovimientoInventario;
    private readonly IRepositorioProducto repositorioProducto;
    private readonly IRepositorioVenta repositorioVenta;
    private readonly IUnidadDeTrabajo unidadDeTrabajo;

    public RegistrarVentaCommandHandler(
        IRepositorioVenta repositorioVenta,
        IRepositorioProducto repositorioProducto,
        IRepositorioMovimientoInventario repositorioMovimientoInventario,
        IRepositorioAsientoContable repositorioAsientoContable,
        IUnidadDeTrabajo unidadDeTrabajo,
        IDomainEventDispatcher domainEventDispatcher
    )
    {
        this.repositorioVenta = repositorioVenta;
        this.repositorioProducto = repositorioProducto;
        this.repositorioMovimientoInventario = repositorioMovimientoInventario;
        this.repositorioAsientoContable = repositorioAsientoContable;
        this.unidadDeTrabajo = unidadDeTrabajo;
        this.domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Result<Guid>> Handle(RegistrarVentaCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Detalles is null || command.Detalles.Count == 0)
        {
            return Result<Guid>.Fallo(new Error("Venta.SinDetalles", "La venta debe tener al menos un detalle."));
        }

        var cuentaDebitoId = ObtenerCuentaDebito(command);
        if (cuentaDebitoId == Guid.Empty)
        {
            return Result<Guid>.Fallo(new Error("Venta.CuentaContableRequerida", "La venta requiere una cuenta contable de debito valida."));
        }

        try
        {
            var venta = Venta.Crear(command.UsuarioId, command.MetodoPago);
            var movimientos = new List<MovimientoInventario>();

            foreach (var detalle in command.Detalles)
            {
                var producto = await repositorioProducto.ObtenerPorIdAsync(detalle.ProductoId, cancellationToken);
                if (producto is null)
                {
                    return Result<Guid>.Fallo(new Error("Producto.NoEncontrado", "Uno de los productos indicados no existe."));
                }

                var cantidadResult = Cantidad.CrearPositiva(detalle.Cantidad);
                if (cantidadResult.EsFallo)
                {
                    return Result<Guid>.Fallo(new Error(cantidadResult.Error.Codigo, cantidadResult.Error.Nombre));
                }

                var precio = new Dinero(detalle.PrecioUnitario, "USD");

                producto.DisminuirStock(cantidadResult.Valor);
                venta.AgregarDetalle(detalle.ProductoId, cantidadResult.Valor, precio);
                movimientos.Add(MovimientoInventario.CrearSalida(detalle.ProductoId, cantidadResult.Valor, $"Venta {venta.Id}"));
            }

            venta.RegistrarEventoVentaRegistrada();

            var asiento = AsientoContable.Crear($"Registro de venta {venta.Id}");
            asiento.AgregarDetalle(cuentaDebitoId, venta.Total.Monto, 0);
            asiento.AgregarDetalle(command.CuentaVentasId, 0, venta.Total.Monto);

            await repositorioVenta.AgregarAsync(venta, cancellationToken);
            await repositorioMovimientoInventario.AgregarRangoAsync(movimientos, cancellationToken);
            await repositorioAsientoContable.AgregarAsync(asiento, cancellationToken);
            await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);

            await domainEventDispatcher.DispatchAsync(venta.ObtenerEventosDominio(), cancellationToken);
            venta.LimpiarEventosDominio();

            return Result<Guid>.Exito(venta.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Fallo(new Error("Venta.DatosInvalidos", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result<Guid>.Fallo(new Error("Venta.OperacionInvalida", ex.Message));
        }
    }

    private static Guid ObtenerCuentaDebito(RegistrarVentaCommand command)
    {
        return command.CuentaCajaBancoId ?? Guid.Empty;
    }
}
