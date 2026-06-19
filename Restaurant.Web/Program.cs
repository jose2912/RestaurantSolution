using Restaurant.Business;
using Restaurant.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 🔗 Inyección de dependencias
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Pedido
builder.Services.AddScoped<PedidoRepository>(sp =>
    new PedidoRepository(connectionString));
builder.Services.AddScoped<PedidoService>();

// DetallePedido
builder.Services.AddScoped<DetallePedidoRepository>(sp =>
    new DetallePedidoRepository(connectionString));
builder.Services.AddScoped<DetallePedidoService>();

// Documento
builder.Services.AddScoped<DocumentoRepository>(sp =>
    new DocumentoRepository(connectionString));
builder.Services.AddScoped<DocumentoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pedidos}/{action=Form}/{id?}")
    .WithStaticAssets();

app.Run();
