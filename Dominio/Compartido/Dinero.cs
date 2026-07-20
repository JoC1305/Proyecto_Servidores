namespace Dominio.Compartido;

public record Dinero(decimal Monto, string Moneda)
{
    public static Dinero operator +(Dinero primero, Dinero segundo)
    {
        if (primero.Moneda != segundo.Moneda)
        {
            throw new InvalidOperationException("Las monedas deben ser iguales.");
        }

        return new Dinero(primero.Monto + segundo.Monto, primero.Moneda);
    }

    public static Dinero operator -(Dinero primero, Dinero segundo)
    {
        if (primero.Moneda != segundo.Moneda)
        {
            throw new InvalidOperationException("Las monedas deben ser iguales.");
        }

        return new Dinero(primero.Monto - segundo.Monto, primero.Moneda);
    }

    public static Dinero Cero(string moneda = "USD") => new(0, moneda);

    public bool EsCero() => Monto == 0;
}
