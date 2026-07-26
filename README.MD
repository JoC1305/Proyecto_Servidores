# 🌾 Agromarket FCVT - Arquitectura, DDD, CQRS y Casos de Uso

Este proyecto implementa un sistema empresarial para la gestión de productos, inventario, compras, ventas y contabilidad utilizando **.NET 10 / C#**, siguiendo **Clean Architecture**, **Domain-Driven Design (DDD)**, el patrón **CQRS** y orquestación distribuida con **.NET Aspire**.

---

## 🏛️ 1. Modelo de Dominio (DDD)

El núcleo del sistema está libre de dependencias externas y sigue estrictamente los patrones tácticos de DDD:

* **Entidades**: Objetos con identidad única (`Guid`) que encapsulan el estado y las reglas del negocio:
  - `Producto`: Administra precios, catálogo y stock.
  - `Categoria`: Agrupa productos según su tipo.
  - `Venta` y `DetalleVenta`: Controla ventas, clientes y métodos de pago.
  - `Compra` y `DetalleCompra`: Registra compras y reabastecimiento a proveedores.
  - `AsientoContable` y `DetalleAsiento`: Registra transacciones contables balanceadas.
  - `MovimientoInventario`: Historial de entradas y salidas de inventario.

* **Objetos de Valor (Value Objects)**: Propiedades inmutables sin identidad propia que encapsulan validaciones de dominio:
  - `Dinero`: Garantiza montos válidos con su tipo de moneda (`USD`).
  - `Cantidad`: Garantiza enteros no negativos o positivos según el contexto.
  - `Nombre`: Valida que las cadenas no sean nulas ni vacías.
  - `CodigoBarras`: Controla la validez del formato de código de barras.
  - `Descripcion`: Encapsula textos descriptivos.

* **Eventos de Dominio**:
  - `VentaRegistrada`: Evento publicado cuando una venta es completada satisfactoriamente, desacoplando la lógica de negocio secundaria a través de `IDomainEventDispatcher`.

* **Principio de Mínimo Privilegio (Encapsulamiento)**:
  - Propiedades con modificadores `private set`.
  - Métodos de comportamiento semántico de dominio en lugar de asignaciones directas (`AumentarStock`, `DisminuirStock`, `AgregarDetalle`, `RegistrarEventoVentaRegistrada`).
  - Colecciones encapsuladas como de solo lectura (`IReadOnlyCollection`).

---

## 🏗️ 2. Arquitectura de Aplicación y CQRS

* **Arquitectura Limpia (Clean Architecture)**:
  - `Dominio`: Sin librerías ni dependencias externas. Contiene entidades y reglas de negocio.
  - `Aplicacion`: Contiene los casos de uso (Commands y Queries) y los contratos de abstracción.
  - `Infraestructura`: Mapeos de Entity Framework Core, repositorios y servicios externos.
  - `API`: Exposición de endpoints RESTful.

* **CQRS (Command Query Responsibility Segregation)**:
  - **Comandos (Escritura / Lógica)**: Implementan `ICommandHandler<TCommand, TResult>` para modificar el estado del sistema.
  - **Consultas (Lectura)**: Implementan `IQueryHandler<TQuery, TResult>` devolviendo DTOs inmutables sin alterar el dominio.

---

## ⚙️ 3. Infraestructura y Persistencia

* **Orquestación con .NET Aspire**:
  - `backend.AppHost`: Gestiona el ciclo de vida del contenedor PostgreSQL (`dbserver.AddDatabase("AgromarketFCVT")`) e inyecta dinámicamente las cadenas de conexión a la API.

* **Estrategia de Persistencia y Migraciones**:
  - Se utiliza **Entity Framework Core 10** con la extensión `Npgsql`.
  - Mapeo de entidades a tablas de PostgreSQL mediante `ContextoDb`.
  - Migraciones automáticas al iniciar la aplicación (`dbContext.Database.MigrateAsync()`).

---

## 🧪 4. Estrategia de Pruebas (Testing)

* **Pruebas de Integración y End-to-End**:
  - Preparado para infraestructura de pruebas con **.NET Aspire** (`Aspire.Hosting.Testing`), permitiendo levantar el entorno orquestado de forma programática y validar endpoints reales sin requerir un entorno físico previamente configurado.

---

## 📡 5. Mecanismo de Interacción (Consumo de API)

El repositorio incluye el archivo listo para ejecutar **[API/API.http](file:///C:/Users/Usuario/Desktop/Universidad/U%20SEM5/Aplicaciones%20para%20el%20servidor%20Web/proyecto_final/Proyecto_Servidores/API/API.http)** con las peticiones ordenadas de cada flujo.

---

## 📋 6. Casos de Uso Implementados (Detalle Lógico y de Código)

### ✍️ Casos de Uso de Escritura (Write)

1. **`CrearCategoria`** (`POST /api/categorias`):
   - *Lógica*: Encapsula `Nombre` y `Descripcion` como Value Objects y registra la entidad `Categoria`.
   - *Código*: [CategoriasController.cs](file:///C:/Users/Usuario/Desktop/Universidad/U%20SEM5/Aplicaciones%20para%20el%20servidor%20Web/proyecto_final/Proyecto_Servidores/API/Controllers/CategoriasController.cs) -> `CrearCategoriaCommandHandler`.

2. **`CrearProducto`** (`POST /api/productos`):
   - *Lógica*: Valida código de barras único, categoría existente, rango de precio e inicia el stock.
   - *Código*: [ProductosController.cs](file:///C:/Users/Usuario/Desktop/Universidad/U%20SEM5/Aplicaciones%20para%20el%20servidor%20Web/proyecto_final/Proyecto_Servidores/API/Controllers/ProductosController.cs) -> `CrearProductoCommandHandler`.

### 📖 Casos de Uso de Lectura (Read)

3. **`ObtenerCategorias`** (`GET /api/categorias`):
   - *Lógica*: Recupera todas las categorías en formato DTO (`CategoriaDto`) utilizando `.AsNoTracking()`.
   - *Código*: `ObtenerCategoriasQueryHandler`.

4. **`ObtenerProductos`** (`GET /api/productos`):
   - *Lógica*: Consulta el catálogo proyectando `ProductoDto` con el cálculo automático de margen de ganancia (`PrecioVenta - PrecioCompra`).
   - *Código*: `ObtenerProductosQueryHandler`.

### ⚙️ Casos de Uso de Lógica de Negocio (Logic)

5. **`RegistrarCompra`** (`POST /api/compras`):
   - *Lógica*: Aumenta automáticamente el stock del producto (`producto.AumentarStock`), registra la compra, guarda el movimiento de entrada en inventario y genera el asiento contable (Inventario vs Proveedores por Pagar).
   - *Código*: [ComprasController.cs](file:///C:/Users/Usuario/Desktop/Universidad/U%20SEM5/Aplicaciones%20para%20el%20servidor%20Web/proyecto_final/Proyecto_Servidores/API/Controllers/ComprasController.cs) -> `RegistrarCompraCommandHandler`.

6. **`RegistrarVenta`** (`POST /api/ventas`):
   - *Lógica*: Comprueba stock disponible y disminuye stock (`producto.DisminuirStock`), registra la venta, guarda el movimiento de salida de inventario, genera el asiento contable y publica el evento de dominio `VentaRegistrada`.
   - *Código*: [VentasController.cs](file:///C:/Users/Usuario/Desktop/Universidad/U%20SEM5/Aplicaciones%20para%20el%20servidor%20Web/proyecto_final/Proyecto_Servidores/API/Controllers/VentasController.cs) -> `RegistrarVentaCommandHandler`.

---

## 🚀 Guía de Inicio Rápido

1. **Ejecutar la API**:
   ```bash
   dotnet run --project API
   ```
   o con .NET Aspire:
   ```bash
   dotnet run --project backend.AppHost
   ```

2. **Ejecutar Pruebas HTTP**:
   Abre [API/API.http](file:///C:/Users/Usuario/Desktop/Universidad/U%20SEM5/Aplicaciones%20para%20el%20servidor%20Web/proyecto_final/Proyecto_Servidores/API/API.http) desde tu IDE (VS Code, Visual Studio o Rider) y ejecuta los 6 endpoints configurados.