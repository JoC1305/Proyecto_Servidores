using Aplicacion.Abstracciones.Persistencia;
using Aplicacion.Comun.CQRS;
using Aplicacion.Comun.Resultados;
using Dominio.Compartido;
using Dominio.Entidades.Compras;
using Dominio.Entidades.Contabilidad;
using Dominio.Entidades.Inventario;

namespace Aplicacion.Compras.RegistrarCompra;

public sealed class RegistrarCompraCommandHandler : ICommandHandler<RegistrarCompraCommand, Guid>
{
    private readonly IRepositorioAsientoContable repositorioAsientoContable;
    private readonly IRepositorioCompra repositorioCompra;
    private readonly IRepositorioMovimientoInventario repositorioMovimientoInventario;
    private readonly IRepositorioProducto repositorioProducto;
    private readonly IUnidadDeTrabajo unidadDeTrabajo;

    public RegistrarCompraCommandHandler(
        IRepositorioCompra repositorioCompra,
        IRepositorioProducto repositorioProducto,
        IRepositorioMovimientoInventario repositorioMovimientoInventario,
        IRepositorioAsientoContable repositorioAsientoContable,
        IUnidadDeTrabajo unidadDeTrabajo
    )
    {
        this.repositorioCompra = repositorioCompra;
        this.repositorioProducto = repositorioProducto;
        this.repositorioMovimientoInventario = repositorioMovimientoInventario;
        this.repositorioAsientoContable = repositorioAsientoContable;
        this.unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Result<Guid>> Handle(RegistrarCompraCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Detalles is null || command.Detalles.Count == 0)
        {
            return Result<Guid>.Fallo(new Error("Compra.SinDetalles", "La compra debe tener al menos un detalle."));
        }

        try
        {
            var compra = Compra.Crear(command.ProveedorId, command.PedidoId);
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

                var precio = new Dinero(detalle.PrecioCompra, "USD");

                producto.AumentarStock(cantidadResult.Valor);
                compra.AgregarDetalle(detalle.ProductoId, cantidadResult.Valor, precio);
                movimientos.Add(MovimientoInventario.CrearEntrada(detalle.ProductoId, cantidadResult.Valor, $"Compra {compra.Id}"));
            }

            var asiento = AsientoContable.Crear($"Registro de compra {compra.Id}");
            asiento.AgregarDetalle(command.CuentaInventarioId, compra.Total.Monto, 0);
            asiento.AgregarDetalle(command.CuentaProveedoresPorPagarId, 0, compra.Total.Monto);

            await repositorioCompra.AgregarAsync(compra, cancellationToken);
            await repositorioMovimientoInventario.AgregarRangoAsync(movimientos, cancellationToken);
            await repositorioAsientoContable.AgregarAsync(asiento, cancellationToken);
            await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);

            return Result<Guid>.Exito(compra.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Fallo(new Error("Compra.DatosInvalidos", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result<Guid>.Fallo(new Error("Compra.OperacionInvalida", ex.Message));
        }
    }
}
