using System.Net;
using System.Net.Http.Json;
using Shouldly; 
using Xunit;

namespace Api.Tests.Categorias;

public class CrearCategoriaTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CrearCategoriaTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CrearCategoria_ConDatosValidos_DebeRetornarCreated()
    {
        // Arrange
        var command = new
        {
            Nombre = "Frutas y Verduras",
            Descripcion = "Productos agrícolas frescos"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/categorias", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Theory]
    [InlineData("", "Descripción válida")]
    [InlineData(null, "Descripción válida")]
    public async Task CrearCategoria_CuandoNombreEsInvalido_DebeLanzarValidationException(string? nombre, string descripcion)
    {
        // Arrange
        var command = new { Nombre = nombre, Descripcion = descripcion };

        // Act
        var response = await _client.PostAsJsonAsync("/api/categorias", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}