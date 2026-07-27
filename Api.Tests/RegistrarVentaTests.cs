using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Xunit;

namespace Api.Tests.Ventas;

public class RegistrarVentaTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegistrarVentaTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegistrarVenta_ConStockSuficiente_DebeDisminuirStockYPublicarEvento()
    {
        // Arrange - Crear Categoria y Producto de prueba
        var catResponse = await _client.PostAsJsonAsync("/api/categorias", new { Nombre = "Categoria Venta", Descripcion = "Desc" });
        catResponse.EnsureSuccessStatusCode();
        var catData = await catResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var categoriaId = catData!["id"];

        var prodResponse = await _client.PostAsJsonAsync("/api/productos", new
        {
            CodigoBarras = "7791234567892",
            Nombre = "Producto Venta",
            PrecioCompra = 10.00,
            PrecioVenta = 15.00,
            IdCategoria = categoriaId,
            Descripcion = "Desc",
            Stock = 50,
            StockMinimo = 2
        });
        prodResponse.EnsureSuccessStatusCode();
        var prodData = await prodResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var productoId = prodData!["id"];

        var command = new
        {
            UsuarioId = Guid.NewGuid(),
            MetodoPago = "Efectivo",
            Detalles = new[]
            {
                new
                {
                    ProductoId = productoId,
                    Cantidad = 5,
                    PrecioUnitario = 15.00
                }
            },
            CuentaVentasId = Guid.NewGuid(),
            CuentaCajaBancoId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ventas", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task RegistrarVenta_SinStockSuficiente_DebeRetornarBadRequest()
    {
        // Arrange - Crear Categoria y Producto de prueba con stock limitado
        var catResponse = await _client.PostAsJsonAsync("/api/categorias", new { Nombre = "Categoria Venta 2", Descripcion = "Desc" });
        catResponse.EnsureSuccessStatusCode();
        var catData = await catResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var categoriaId = catData!["id"];

        var prodResponse = await _client.PostAsJsonAsync("/api/productos", new
        {
            CodigoBarras = "7791234567893",
            Nombre = "Producto Venta 2",
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
            UsuarioId = Guid.NewGuid(),
            MetodoPago = "Efectivo",
            Detalles = new[]
            {
                new
                {
                    ProductoId = productoId,
                    Cantidad = 999999, // Supera stock
                    PrecioUnitario = 15.00
                }
            },
            CuentaVentasId = Guid.NewGuid(),
            CuentaCajaBancoId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ventas", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}