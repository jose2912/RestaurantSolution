using Restaurant.Business;
using Restaurant.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// OpenAPI/Swagger
builder.Services.AddOpenApi();

// 🔗 Inyección de dependencias
// Connection string desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registramos repositorios y servicios
builder.Services.AddScoped<PedidoRepository>(sp =>
    new PedidoRepository(connectionString));
builder.Services.AddScoped<PedidoService>();

builder.Services.AddScoped<DocumentoRepository>(sp =>
    new DocumentoRepository(connectionString));
builder.Services.AddScoped<DocumentoService>();

builder.Services.AddScoped<DetallePedidoRepository>(sp =>
    new DetallePedidoRepository(connectionString));
builder.Services.AddScoped<DetallePedidoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
