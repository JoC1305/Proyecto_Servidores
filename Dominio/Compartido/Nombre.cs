using Dominio.Comun;

namespace Dominio.Compartido;

public sealed record Nombre
{
    public static readonly Error Invalido = new(
        "Nombre.Invalido",
        "El nombre no puede estar vacío."
    );

    public Nombre(string valor) => Valor = valor;

    public string Valor { get; init; }

    public static Resultado<Nombre> Crear(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return Resultado.Fallo<Nombre>(Invalido);
        }

        return new Nombre(valor.Trim());
    }
}
