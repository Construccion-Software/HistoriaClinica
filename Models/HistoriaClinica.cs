using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HistoriasClinicas.Api.Models
{
    [BsonIgnoreExtraElements]
    public class HistoriaClinica
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string CedulaPaciente { get; set; } = string.Empty;
        public Dictionary<string, RegistroClinico> Historico { get; set; } = new Dictionary<string, RegistroClinico>();
        public DateTime? FechaUltimaActualizacion { get; set; }
    }
}