using System.Collections.Generic;
using System.Linq;
using HistoriasClinicas.Api.Models;
using HistoriasClinicas.Api.Models.Enums;

namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    public class AyudaDiagnosticaExcluyentesRule : IValidationRule
    {
        public string NombreRegla => "Ayuda Diagnóstica Excluyentes";

        public async Task<Dictionary<string, List<string>>> Validar(RegistroClinico registro, RegistroValidacionContext contexto)
        {
            var errors = new Dictionary<string, List<string>>();

            if (registro.AyudasDiagnosticas == null || !registro.AyudasDiagnosticas.Any())
                return errors;

            var hayAyudasPendientes = registro.AyudasDiagnosticas
                .Any(a => a.Estado == EstadoAyudaDiagnostica.Pendiente || a.Estado == EstadoAyudaDiagnostica.ConResultados);

            if (hayAyudasPendientes)
            {
                if (registro.Medicamentos != null && registro.Medicamentos.Any())
                {
                    AddError(errors, "medicamentos",
                        "No se pueden recetar medicamentos cuando hay ayudas diagnósticas pendientes de procesar.");
                }

                if (registro.Procedimientos != null && registro.Procedimientos.Any())
                {
                    AddError(errors, "procedimientos",
                        "No se pueden recetar procedimientos cuando hay ayudas diagnósticas pendientes de procesar.");
                }
            }

            return await Task.FromResult(errors);
        }

        private void AddError(Dictionary<string, List<string>> errors, string key, string message)
        {
            if (!errors.ContainsKey(key))
                errors[key] = new List<string>();

            errors[key].Add(message);
        }
    }
}
