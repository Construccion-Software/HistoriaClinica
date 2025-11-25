using System;
using System.Collections.Generic;
using HistoriasClinicas.Api.Models.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace HistoriasClinicas.Api.Models
{
    [BsonIgnoreExtraElements]
    public class RegistroClinico
    {
        public DateTime FechaAtencion { get; set; }
        public string CedulaMedico { get; set; } = string.Empty;
        public string? MotivoConsulta { get; set; }
        public string? Sintomatologia { get; set; }
        public string? Diagnostico { get; set; }
        public EstadoRegistroClinico Estado { get; set; } = EstadoRegistroClinico.EnProceso;
        public List<AyudaDiagnostica>? AyudasDiagnosticas { get; set; }
        public List<ProcedimientoRealizado>? Procedimientos { get; set; }
        public List<MedicamentoRecetado>? Medicamentos { get; set; }
        public string? Notas { get; set; }
    }
}
