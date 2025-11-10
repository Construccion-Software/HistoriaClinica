using HistoriasClinicas.Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using HistoriasClinicas.Api.Repositories;
using HistoriasClinicas.Api.Services;
using DotNetEnv;

// Cargar variables de entorno desde archivo .env (solo si no existen)
if (!File.Exists(".env.local"))
{
    Env.Load(".env");
}

var builder = WebApplication.CreateBuilder(args);

// Configurar MongoDB con variables de entorno o fallback a appsettings
builder.Services.Configure<MongoDbSettings>(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") 
        ?? builder.Configuration["MongoDbSettings:ConnectionString"];
    options.DatabaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME")
        ?? builder.Configuration["MongoDbSettings:DatabaseName"];
    options.CollectionName = Environment.GetEnvironmentVariable("MONGODB_COLLECTION_NAME")
        ?? builder.Configuration["MongoDbSettings:CollectionName"];
});


builder.Services.AddSingleton(sp =>
     sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(
    sp => new MongoClient(sp.GetRequiredService<MongoDbSettings>().ConnectionString));

builder.Services.AddSingleton<HistoriaClinicaRepository>();
builder.Services.AddScoped<HistoriaClinicaService>();

// Agregar CORS para permitir comunicación entre microservicios
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Usar CORS
app.UseCors("AllowAll");

// Habilitar Swagger siempre (útil para testing)
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
