namespace Dominio.Comun;

public record Error(string Codigo, string Nombre)
{
    public static readonly Error Ninguno = new(string.Empty, string.Empty);

    public static readonly Error ValorNulo = new("Error.ValorNulo", "Se proporcionó un valor nulo");
}
