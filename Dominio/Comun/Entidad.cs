namespace Dominio.Comun;

public abstract class Entidad
{
    private readonly List<IDomainEvent> eventosDominio = [];

    public Guid Id { get; init; } = Guid.NewGuid();

    public IReadOnlyList<IDomainEvent> ObtenerEventosDominio()
    {
        return eventosDominio.ToList();
    }

    public void LimpiarEventosDominio()
    {
        eventosDominio.Clear();
    }

    protected void RegistrarEventoDominio(IDomainEvent eventoDominio)
    {
        eventosDominio.Add(eventoDominio);
    }
}
