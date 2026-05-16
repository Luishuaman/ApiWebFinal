using CrudProductos.Middlewares;
using CrudProductos.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseLoggerMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRUD Productos v1");
        c.RoutePrefix = string.Empty;
    });
}

var productos = new List<Producto>();
int nextId = 1;

app.MapGet("/productos", () => Results.Ok(productos))
    .WithName("GetProductos")
    .WithTags("Productos")
    .WithSummary("Obtiene todos los productos");

app.MapGet("/productos/{id}", (int id) =>
{
    var p = productos.FirstOrDefault(x => x.Id == id);
    return p is not null ? Results.Ok(p) : Results.NotFound(new { mensaje = $"Producto {id} no encontrado" });
})
.WithName("GetProductoById")
.WithTags("Productos")
.WithSummary("Obtiene un producto por ID");

app.MapPost("/productos", (Producto nuevo) =>
{
    nuevo.Id = nextId++;
    productos.Add(nuevo);
    return Results.Created($"/productos/{nuevo.Id}", nuevo);
})
.WithName("CreateProducto")
.WithTags("Productos")
.WithSummary("Crea un nuevo producto");

app.MapPut("/productos/{id}", (int id, Producto actualizado) =>
{
    var p = productos.FirstOrDefault(x => x.Id == id);
    if (p is null) return Results.NotFound(new { mensaje = $"Producto {id} no encontrado" });

    p.Nombre = actualizado.Nombre;
    p.Precio = actualizado.Precio;
    p.Stock  = actualizado.Stock;

    return Results.Ok(p);
})
.WithName("UpdateProducto")
.WithTags("Productos")
.WithSummary("Actualiza un producto existente");

app.MapDelete("/productos/{id}", (int id) =>
{
    var p = productos.FirstOrDefault(x => x.Id == id);
    if (p is null) return Results.NotFound(new { mensaje = $"Producto {id} no encontrado" });

    productos.Remove(p);
    return Results.NoContent();
})
.WithName("DeleteProducto")
.WithTags("Productos")
.WithSummary("Elimina un producto");

await app.RunAsync();