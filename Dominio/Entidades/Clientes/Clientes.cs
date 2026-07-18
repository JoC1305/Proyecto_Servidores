namespace Dominio.Entidades.Clientes;

public class Cliente
{
    public Guid Id { get; private set; }

    public string Cedula { get; private set; }

    public string Nombre { get; private set; }

    public string Telefono { get; private set; }

    public string Correo { get; private set; }
}