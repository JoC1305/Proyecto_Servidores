using Dominio.Comun;

namespace Dominio.Entidades.Clientes;

public sealed record Cedula
{
    public static readonly Error Invalida = new(
        "Cedula.Invalida",
        "La cédula no puede estar vacía."
    );

    private Cedula(string valor) => Valor = valor;

    public string Valor { get; init; }

    public static Resultado<Cedula> Crear(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return Resultado.Fallo<Cedula>(Invalida);
        }

        return new Cedula(valor.Trim());
    }
}
