using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Xunit;

namespace Api.Tests.Productos;

public class CrearProductoTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CrearProductoTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CrearProducto_ConCodigoBarrasValido_DebeRetornarCreated()
    {
        // Arrange - Crear primero una categoría válida
        var catResponse = await _client.PostAsJsonAsync("/api/categorias", new
        {
            Nombre = "Categoría Frutas",
            Descripcion = "Descripción"
        });
        catResponse.EnsureSuccessStatusCode();
        var catData = await catResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var categoriaId = catData!["id"];

        var command = new
        {
            CodigoBarras = "7791234567890",
            Nombre = "Manzana Crisp",
            PrecioCompra = 1.20,
            PrecioVenta = 2.00,
            IdCategoria = categoriaId,
            Descripcion = "Fresca",
            Stock = 50,
            StockMinimo = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/productos", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}