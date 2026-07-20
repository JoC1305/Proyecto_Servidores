using Aplicacion.Abstracciones.Persistencia;
using Aplicacion.Comun.CQRS;
using Aplicacion.Comun.Resultados;
using Dominio.Compartido;
using Dominio.Entidades.Inventario;

namespace Aplicacion.Productos.CrearProducto;

public sealed class CrearProductoCommandHandler : ICommandHandler<CrearProductoCommand, Guid>
{
    private readonly IRepositorioCategoria repositorioCategoria;
    private readonly IRepositorioProducto repositorioProducto;
    private readonly IUnidadDeTrabajo unidadDeTrabajo;

    public CrearProductoCommandHandler(
        IRepositorioProducto repositorioProducto,
        IRepositorioCategoria repositorioCategoria,
        IUnidadDeTrabajo unidadDeTrabajo
    )
    {
        this.repositorioProducto = repositorioProducto;
        this.repositorioCategoria = repositorioCategoria;
        this.unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Result<Guid>> Handle(CrearProductoCommand command, CancellationToken cancellationToken = default)
    {
        if (await repositorioProducto.ExisteCodigoBarrasAsync(command.CodigoBarras, cancellationToken))
        {
            return Result<Guid>.Fallo(new Error("Producto.CodigoDuplicado", "Ya existe un producto con ese codigo de barras."));
        }

        var categoria = await repositorioCategoria.ObtenerPorIdAsync(command.IdCategoria, cancellationToken);
        if (categoria is null)
        {
            return Result<Guid>.Fallo(new Error("Categoria.NoEncontrada", "La categoria indicada no existe."));
        }

        var codigoBarrasResult = CodigoBarras.Crear(command.CodigoBarras);
        if (codigoBarrasResult.EsFallo)
        {
            return Result<Guid>.Fallo(new Error(codigoBarrasResult.Error.Codigo, codigoBarrasResult.Error.Nombre));
        }

        var nombreResult = Nombre.Crear(command.Nombre);
        if (nombreResult.EsFallo)
        {
            return Result<Guid>.Fallo(new Error(nombreResult.Error.Codigo, nombreResult.Error.Nombre));
        }

        var stockResult = Cantidad.Crear(command.Stock);
        if (stockResult.EsFallo)
        {
            return Result<Guid>.Fallo(new Error(stockResult.Error.Codigo, stockResult.Error.Nombre));
        }

        var stockMinimoResult = Cantidad.Crear(command.StockMinimo);
        if (stockMinimoResult.EsFallo)
        {
            return Result<Guid>.Fallo(new Error(stockMinimoResult.Error.Codigo, stockMinimoResult.Error.Nombre));
        }

        try
        {
            var producto = Producto.Crear(
                codigoBarrasResult.Valor,
                nombreResult.Valor,
                new Dinero(command.PrecioCompra, "USD"),
                new Dinero(command.PrecioVenta, "USD"),
                command.IdCategoria,
                new Descripcion(command.Descripcion ?? string.Empty),
                stockResult.Valor,
                stockMinimoResult.Valor
            );

            await repositorioProducto.AgregarAsync(producto, cancellationToken);
            await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);

            return Result<Guid>.Exito(producto.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Fallo(new Error("Producto.DatosInvalidos", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result<Guid>.Fallo(new Error("Producto.OperacionInvalida", ex.Message));
        }
    }
}
