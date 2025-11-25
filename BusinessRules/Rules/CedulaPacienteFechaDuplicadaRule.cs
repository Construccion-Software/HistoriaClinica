using System.Collections.Generic;
using System.Threading.Tasks;
using HistoriasClinicas.Api.Models;

namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    public class CedulaPacienteFechaDuplicadaRule : IValidationRule
    {
        public string NombreRegla => "Cédula-Paciente Fecha Duplicada";

        public Task<Dictionary<string, List<string>>> Validar(RegistroClinico registro, RegistroValidacionContext contexto)
        {
            var errors = new Dictionary<string, List<string>>();

            if (!contexto.EsActualizacion && contexto.HistoriaExistente != null)
            {
                if (contexto.HistoriaExistente.Historico.ContainsKey(contexto.FechaClave))
                {
                    AddError(errors, "fechaAtencion",
                        $"Ya existe un registro para la fecha {registro.FechaAtencion:yyyy-MM-dd HH:mm}. El histórico no admite duplicados.");
                }
            }

            return Task.FromResult(errors);
        }

        private void AddError(Dictionary<string, List<string>> errors, string key, string message)
        {
            if (!errors.ContainsKey(key))
                errors[key] = new List<string>();

            errors[key].Add(message);
        }
    }
}