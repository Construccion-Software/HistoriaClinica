using HistoriasClinicas.Api.Models;

namespace HistoriasClinicas.Api.BusinessRules
{
    public class RegistroValidacionContext
    {
        public string CedulaPaciente { get; set; } = string.Empty;
        public HistoriaClinica? HistoriaExistente { get; set; }
        public bool EsActualizacion { get; set; }
        public string FechaClave { get; set; } = string.Empty;
    }
}
