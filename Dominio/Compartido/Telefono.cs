using Dominio.Comun;

namespace Dominio.Compartido;

public sealed record Telefono
{
    public static readonly Error Invalido = new(
        "Telefono.Invalido",
        "El teléfono no puede estar vacío."
    );

    private Telefono(string valor) => Valor = valor;

    public string Valor { get; init; }

    public static Resultado<Telefono> Crear(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return Resultado.Fallo<Telefono>(Invalido);
        }

        return new Telefono(valor.Trim());
    }
}
