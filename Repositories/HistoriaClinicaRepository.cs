using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HistoriasClinicas.Api.Models;
using HistoriasClinicas.Api.Settings;
using MongoDB.Driver;

namespace HistoriasClinicas.Api.Repositories
{
    public class HistoriaClinicaRepository
    {
        private readonly IMongoCollection<HistoriaClinica> _historiasCollection;

        public HistoriaClinicaRepository(IMongoClient mongoClient, MongoDbSettings settings)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _historiasCollection = database.GetCollection<HistoriaClinica>(settings.CollectionName);
            CrearIndices();
        }

        private void CrearIndices()
        {
            try
            {
                var indexKeysDefinition = Builders<HistoriaClinica>.IndexKeys.Ascending(h => h.CedulaPaciente);
                var indexOptions = new CreateIndexOptions { Unique = true };
                var indexModel = new CreateIndexModel<HistoriaClinica>(indexKeysDefinition, indexOptions);
                _historiasCollection.Indexes.CreateOne(indexModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear índices: {ex.Message}");
            }
        }

        public Task<List<HistoriaClinica>> GetAllAsync()
        {
            return _historiasCollection.Find(_ => true).ToListAsync();
        }

        public async Task<HistoriaClinica?> GetByCedulaAsync(string cedula)
        {
            return await _historiasCollection
                .Find(h => h.CedulaPaciente == cedula)
                .FirstOrDefaultAsync();
        }

        public async Task UpsertRegistroAsync(string cedulaPaciente, string fechaClave, RegistroClinico registro)
        {
            var filter = Builders<HistoriaClinica>.Filter.Eq(h => h.CedulaPaciente, cedulaPaciente);
            var update = Builders<HistoriaClinica>.Update
                .Set(h => h.CedulaPaciente, cedulaPaciente)
                .Set($"{nameof(HistoriaClinica.Historico)}.{fechaClave}", registro)
                .Set(h => h.FechaUltimaActualizacion, DateTime.UtcNow);

            await _historiasCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }
    }
}
