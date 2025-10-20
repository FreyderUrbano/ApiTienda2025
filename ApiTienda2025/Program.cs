using ApiTienda2025.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
    
/*
Registra tu TiendaContext dentro del contenedor de dependencias de ASP.NET Core.
Configura EF Core para usar PostgreSQL como proveedor. Obtiene la cadena de 
conexión desde appsettings.json (clave "PostgresConnection"). Permite que tus 
controladores o servicios reciban el contexto automáticamente por inyección de dependencias.
 */
builder.Services.AddDbContext<TiendaContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//CODIGO PARA VERIFICAR CONEXION EXITOSA CON POSTGRESS
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    try
    {
        db.Database.OpenConnection();
        Console.WriteLine("✅ Conexión a PostgreSQL exitosa.");
        db.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error al conectar con la base de datos:");
        Console.WriteLine(ex.Message);
    }
}

app.Run();
