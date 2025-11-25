using System.Collections.Generic;
using System.Linq;
using HistoriasClinicas.Api.Models;

namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    public class MedicamentosAsociadosRule : IValidationRule
    {
        public string NombreRegla => "Medicamentos Asociados a Misma Orden";

        public async Task<Dictionary<string, List<string>>> Validar(RegistroClinico registro, RegistroValidacionContext contexto)
        {
            var errors = new Dictionary<string, List<string>>();

            if (registro.Medicamentos == null || registro.Medicamentos.Count <= 1)
                return errors;

            var ordenesDistintas = registro.Medicamentos
                .Select(m => m.NumeroOrden)
                .Distinct()
                .ToList();

            if (ordenesDistintas.Count > 1)
            {
                var ordenes = string.Join(", ", ordenesDistintas);
                AddError(errors, "medicamentos",
                    $"Se encontraron múltiples órdenes ({ordenes}) en los medicamentos recetados. Todos deben compartir la misma orden.");
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
