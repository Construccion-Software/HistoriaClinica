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
        }

        public async Task<List<HistoriaClinica>> GetAllAsync() =>
            await _historiasCollection.Find(_ => true).ToListAsync();

        public async Task<HistoriaClinica> GetByIdAsync(string id) =>
            await _historiasCollection.Find(h => h.Id == id).FirstOrDefaultAsync();

        public async Task<HistoriaClinica> GetByCedulaAndFechaAsync(string cedula, DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fechaInicio.AddDays(1);
            
            return await _historiasCollection.Find(h => 
                h.CedulaPaciente == cedula && 
                h.FechaAtencion >= fechaInicio && 
                h.FechaAtencion < fechaFin
            ).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(HistoriaClinica historia) =>
            await _historiasCollection.InsertOneAsync(historia);

        public async Task UpdateAsync(string id, HistoriaClinica historia) =>
            await _historiasCollection.ReplaceOneAsync(h => h.Id == id, historia);

        public async Task DeleteAsync(string id) =>
            await _historiasCollection.DeleteOneAsync(h => h.Id == id);
    }
}
