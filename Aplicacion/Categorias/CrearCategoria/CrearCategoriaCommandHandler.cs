using Aplicacion.Abstracciones.Persistencia;
using Aplicacion.Comun.CQRS;
using Aplicacion.Comun.Resultados;
using Dominio.Compartido;
using Dominio.Entidades.Categoria;
using Dominio.Entidades.Inventario;

namespace Aplicacion.Categorias.CrearCategoria;

public sealed class CrearCategoriaCommandHandler : ICommandHandler<CrearCategoriaCommand, Guid>
{
    private readonly IRepositorioCategoria repositorioCategoria;
    private readonly IUnidadDeTrabajo unidadDeTrabajo;

    public CrearCategoriaCommandHandler(IRepositorioCategoria repositorioCategoria, IUnidadDeTrabajo unidadDeTrabajo)
    {
        this.repositorioCategoria = repositorioCategoria;
        this.unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Result<Guid>> Handle(CrearCategoriaCommand command, CancellationToken cancellationToken = default)
    {
        var nombreResult = Nombre.Crear(command.Nombre);
        if (nombreResult.EsFallo)
        {
            return Result<Guid>.Fallo(new Error(nombreResult.Error.Codigo, nombreResult.Error.Nombre));
        }

        try
        {
            var categoria = Categoria.Crear(
                nombreResult.Valor,
                new Descripcion(command.Descripcion ?? string.Empty)
            );

            await repositorioCategoria.AgregarAsync(categoria, cancellationToken);
            await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);

            return Result<Guid>.Exito(categoria.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Fallo(new Error("Categoria.DatosInvalidos", ex.Message));
        }
    }
}
