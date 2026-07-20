using Dominio.Compartido;
using Dominio.Entidades.Categoria;
using Dominio.Entidades.Compras;
using Dominio.Entidades.Contabilidad;
using Dominio.Entidades.Inventario;
using Dominio.Entidades.Ventas;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Persistencia;

public sealed class ContextoDb : DbContext
{
    public ContextoDb(DbContextOptions<ContextoDb> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Producto> Productos => Set<Producto>();

    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();

    public DbSet<Venta> Ventas => Set<Venta>();

    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();

    public DbSet<Compra> Compras => Set<Compra>();

    public DbSet<DetalleCompra> DetallesCompra => Set<DetalleCompra>();

    public DbSet<AsientoContable> AsientosContables => Set<AsientoContable>();

    public DbSet<DetalleAsiento> DetallesAsiento => Set<DetalleAsiento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias");
            entity.HasKey(categoria => categoria.Id);
            entity.Property(categoria => categoria.Nombre)
                .HasConversion(v => v.Valor, v => new Nombre(v))
                .HasMaxLength(120).IsRequired();
            entity.Property(categoria => categoria.Descripcion)
                .HasConversion(v => v.Valor, v => new Descripcion(v))
                .HasMaxLength(500);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasKey(producto => producto.Id);
            entity.Property(producto => producto.CodigoBarras)
                .HasConversion(v => v.Valor, v => new CodigoBarras(v))
                .HasColumnName("CodigoBarras").HasMaxLength(80).IsRequired();
            entity.HasIndex(producto => producto.CodigoBarras).IsUnique();
            entity.Property(producto => producto.Nombre)
                .HasConversion(v => v.Valor, v => new Nombre(v))
                .HasMaxLength(160).IsRequired();
            entity.Property(producto => producto.Descripcion)
                .HasConversion(v => v.Valor, v => new Descripcion(v))
                .HasMaxLength(500);
            entity.Property(producto => producto.PrecioCompra)
                .HasConversion(v => v.Monto, v => new Dinero(v, "USD"))
                .HasPrecision(12, 2);
            entity.Property(producto => producto.PrecioVenta)
                .HasConversion(v => v.Monto, v => new Dinero(v, "USD"))
                .HasPrecision(12, 2);
            entity.Property(producto => producto.Stock)
                .HasConversion(v => v.Valor, v => new Cantidad(v));
            entity.Property(producto => producto.StockMinimo)
                .HasConversion(v => v.Valor, v => new Cantidad(v));
            entity.Ignore(producto => producto.Ganancia);
            entity.HasOne<Categoria>().WithMany().HasForeignKey(producto => producto.IdCategoria).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            entity.ToTable("MovimientosInventario");
            entity.HasKey(movimiento => movimiento.Id);
            entity.Property(movimiento => movimiento.Tipo).HasConversion<string>().HasMaxLength(20);
            entity.Property(movimiento => movimiento.Cantidad)
                .HasConversion(v => v.Valor, v => new Cantidad(v));
            entity.Property(movimiento => movimiento.Observacion).HasMaxLength(500);
            entity.HasOne<Producto>().WithMany().HasForeignKey(movimiento => movimiento.ProductoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.ToTable("Ventas");
            entity.HasKey(venta => venta.Id);
            entity.Ignore(venta => venta.Detalles);
            entity.Property(venta => venta.Total)
                .HasConversion(v => v.Monto, v => new Dinero(v, "USD"))
                .HasPrecision(12, 2);
            entity.Property(venta => venta.MetodoPago).HasConversion<string>().HasMaxLength(40);
            entity.HasMany(venta => venta.Detalles).WithOne().HasForeignKey(detalle => detalle.VentaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.ToTable("DetallesVenta");
            entity.HasKey(detalle => detalle.Id);
            entity.Property(detalle => detalle.Cantidad)
                .HasConversion(v => v.Valor, v => new Cantidad(v));
            entity.Property(detalle => detalle.PrecioUnitario)
                .HasConversion(v => v.Monto, v => new Dinero(v, "USD"))
                .HasPrecision(12, 2);
            entity.Ignore(detalle => detalle.Subtotal);
            entity.HasOne<Producto>().WithMany().HasForeignKey(detalle => detalle.ProductoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.ToTable("Compras");
            entity.HasKey(compra => compra.Id);
            entity.Property(compra => compra.Total)
                .HasConversion(v => v.Monto, v => new Dinero(v, "USD"))
                .HasPrecision(12, 2);
            entity.HasMany(compra => compra.Detalles).WithOne().HasForeignKey(detalle => detalle.CompraId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.ToTable("DetallesCompra");
            entity.HasKey(detalle => detalle.Id);
            entity.Property(detalle => detalle.Cantidad)
                .HasConversion(v => v.Valor, v => new Cantidad(v));
            entity.Property(detalle => detalle.PrecioCompra)
                .HasConversion(v => v.Monto, v => new Dinero(v, "USD"))
                .HasPrecision(12, 2);
            entity.Ignore(detalle => detalle.Subtotal);
            entity.HasOne<Producto>().WithMany().HasForeignKey(detalle => detalle.ProductoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AsientoContable>(entity =>
        {
            entity.ToTable("AsientosContables");
            entity.HasKey(asiento => asiento.Id);
            entity.Property(asiento => asiento.Descripcion).HasMaxLength(500).IsRequired();
            entity.HasMany(asiento => asiento.Detalles).WithOne().HasForeignKey(detalle => detalle.AsientoContableId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DetalleAsiento>(entity =>
        {
            entity.ToTable("DetallesAsiento");
            entity.HasKey(detalle => detalle.Id);
            entity.Property(detalle => detalle.Debe).HasPrecision(12, 2);
            entity.Property(detalle => detalle.Haber).HasPrecision(12, 2);
        });
    }
}
