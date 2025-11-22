using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;



namespace HistoriasClinicas.Api.Models
{
    public class HistoriaClinica
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public  string CedulaPaciente { get; set; }
        public DateTime FechaAtencion { get; set; }
        public  string? CedulaMedico { get; set; }
        public  string? MotivoConsulta { get; set; }
        public  string? Sintomatologia { get; set; }
        public  string? Diagnostico { get; set; }
        public  List<MedicamentoRecetado>? Medicamentos { get; set; }
        public  List<ProcedimientoRealizado>? Procedimientos { get; set; }
        public  List<AyudaDiagnostica>? AyudasDiagnosticas { get; set; }
    }
}