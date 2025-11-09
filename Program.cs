using HistoriasClinicas.Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using HistoriasClinicas.Api.Repositories;
using HistoriasClinicas.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));


builder.Services.AddSingleton(sp =>
     sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(
    sp => new MongoClient(sp.GetRequiredService<MongoDbSettings>().ConnectionString));

builder.Services.AddSingleton<HistoriaClinicaRepository>();
builder.Services.AddScoped<HistoriaClinicaService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
