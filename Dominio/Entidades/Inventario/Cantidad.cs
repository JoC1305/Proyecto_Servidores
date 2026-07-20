using Dominio.Comun;

namespace Dominio.Entidades.Inventario;

public sealed record Cantidad
{
    public static readonly Error Invalida = new(
        "Cantidad.Invalida",
        "La cantidad debe ser mayor o igual a cero."
    );

    public static readonly Cantidad Cero = new(0);

    public Cantidad(int valor) => Valor = valor;

    public int Valor { get; init; }

    public static Resultado<Cantidad> Crear(int valor)
    {
        if (valor < 0)
        {
            return Resultado.Fallo<Cantidad>(Invalida);
        }

        return new Cantidad(valor);
    }

    public static Resultado<Cantidad> CrearPositiva(int valor)
    {
        if (valor <= 0)
        {
            return Resultado.Fallo<Cantidad>(new Error(
                "Cantidad.DebeSerPositiva",
                "La cantidad debe ser mayor que cero."
            ));
        }

        return new Cantidad(valor);
    }
}
