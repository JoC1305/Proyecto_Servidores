using Dominio.Comun;

namespace Dominio.Entidades.Inventario;

public sealed record CodigoBarras
{
    public static readonly Error Invalido = new(
        "CodigoBarras.Invalido",
        "El código de barras no puede estar vacío."
    );

    public CodigoBarras(string valor) => Valor = valor;

    public string Valor { get; init; }

    public static Resultado<CodigoBarras> Crear(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return Resultado.Fallo<CodigoBarras>(Invalido);
        }

        return new CodigoBarras(valor.Trim());
    }
}
