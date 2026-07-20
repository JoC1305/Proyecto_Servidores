using Aplicacion.Abstracciones.Persistencia;
using Aplicacion.Comun.CQRS;
using Aplicacion.Comun.Resultados;

namespace Aplicacion.Categorias.ObtenerCategorias;

public sealed class ObtenerCategoriasQueryHandler : IQueryHandler<ObtenerCategoriasQuery, IReadOnlyCollection<CategoriaDto>>
{
    private readonly IRepositorioCategoria repositorioCategoria;

    public ObtenerCategoriasQueryHandler(IRepositorioCategoria repositorioCategoria)
    {
        this.repositorioCategoria = repositorioCategoria;
    }

    public async Task<Result<IReadOnlyCollection<CategoriaDto>>> Handle(
        ObtenerCategoriasQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var categorias = await repositorioCategoria.ObtenerTodasAsync(cancellationToken);
        var resultado = categorias
            .Select(categoria => new CategoriaDto(
                categoria.Id,
                categoria.Nombre.Valor,
                categoria.Descripcion.Valor))
            .ToArray();

        return Result<IReadOnlyCollection<CategoriaDto>>.Exito(resultado);
    }
}
