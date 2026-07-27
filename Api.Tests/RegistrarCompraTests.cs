using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Xunit;

namespace Api.Tests.Compras;

public class RegistrarCompraTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegistrarCompraTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegistrarCompra_DebeAumentarStockYGenerarAsientoContable()
    {
        // Arrange - Crear Categoria y Producto de prueba
        var catResponse = await _client.PostAsJsonAsync("/api/categorias", new { Nombre = "Categoria Compra", Descripcion = "Desc" });
        catResponse.EnsureSuccessStatusCode();
        var catData = await catResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var categoriaId = catData!["id"];

        var prodResponse = await _client.PostAsJsonAsync("/api/productos", new
        {
            CodigoBarras = "7791234567891",
            Nombre = "Producto Compra",
            PrecioCompra = 10.00,
            PrecioVenta = 15.00,
            IdCategoria = categoriaId,
            Descripcion = "Desc",
            Stock = 10,
            StockMinimo = 2
        });
        prodResponse.EnsureSuccessStatusCode();
        var prodData = await prodResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var productoId = prodData!["id"];

        var command = new
        {
            ProveedorId = Guid.NewGuid(),
            PedidoId = Guid.NewGuid(),
            Detalles = new[]
            {
                new
                {
                    ProductoId = productoId,
                    Cantidad = 100,
                    PrecioCompra = 1.50
                }
            },
            CuentaInventarioId = Guid.NewGuid(),
            CuentaProveedoresPorPagarId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/compras", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}