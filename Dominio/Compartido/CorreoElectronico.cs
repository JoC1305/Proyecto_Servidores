using Dominio.Comun;

namespace Dominio.Compartido;

public sealed record CorreoElectronico
{
    public static readonly Error Invalido = new(
        "CorreoElectronico.Invalido",
        "El correo electrónico no puede estar vacío."
    );

    private CorreoElectronico(string valor) => Valor = valor;

    public string Valor { get; init; }

    public static Resultado<CorreoElectronico> Crear(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return Resultado.Fallo<CorreoElectronico>(Invalido);
        }

        return new CorreoElectronico(valor.Trim().ToLowerInvariant());
    }
}
