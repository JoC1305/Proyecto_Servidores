namespace Dominio.Entidades.Inventario;

public class Producto
{
    public Guid Id { get; private set; }
    public string Codigo_Barras { get; private set; }
    public string Nombre { get; private set; }
    public decimal PrecioCompra { get; private set; }
    public decimal PrecioVenta { get; private set; }
    public string Descripcion { get; private set; }
    public int Stock { get; private set; }
    public int StockMinimo { get; private set; }
    public bool Activo { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public Guid IdCategoria { get; private set; }
    public decimal Ganancia => PrecioVenta - PrecioCompra;
}