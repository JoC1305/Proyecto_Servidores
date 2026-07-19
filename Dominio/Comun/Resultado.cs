using System.Diagnostics.CodeAnalysis;

namespace Dominio.Comun;

public class Resultado
{
    protected internal Resultado(bool esExitoso, Error error)
    {
        if (esExitoso && error != Error.Ninguno)
        {
            throw new InvalidOperationException();
        }

        if (!esExitoso && error == Error.Ninguno)
        {
            throw new InvalidOperationException();
        }

        EsExitoso = esExitoso;
        Error = error;
    }

    public bool EsExitoso { get; }

    public bool EsFallo => !EsExitoso;

    public Error Error { get; }

    public static Resultado Exito() => new(true, Error.Ninguno);

    public static Resultado Fallo(Error error) => new(false, error);

    public static Resultado<TValor> Exito<TValor>(TValor valor) => new(valor, true, Error.Ninguno);

    public static Resultado<TValor> Fallo<TValor>(Error error) => new(default, false, error);

    public static Resultado<TValor> Crear<TValor>(TValor? valor) =>
        valor is not null ? Exito(valor) : Fallo<TValor>(Error.ValorNulo);
}

public class Resultado<TValor> : Resultado
{
    private readonly TValor? _valor;

    protected internal Resultado(TValor? valor, bool esExitoso, Error error)
        : base(esExitoso, error)
    {
        _valor = valor;
    }

    [NotNull]
    public TValor Valor =>
        EsExitoso
            ? _valor!
            : throw new InvalidOperationException(
                "No se puede acceder al valor de un resultado fallido."
            );

    public static implicit operator Resultado<TValor>(TValor? valor) => Crear(valor);
}
