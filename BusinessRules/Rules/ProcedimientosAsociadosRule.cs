using System.Collections.Generic;
using System.Linq;
using HistoriasClinicas.Api.Models;

namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    public class ProcedimientosAsociadosRule : IValidationRule
    {
        public string NombreRegla => "Procedimientos Asociados a Misma Orden";

        public async Task<Dictionary<string, List<string>>> Validar(RegistroClinico registro, RegistroValidacionContext contexto)
        {
            var errors = new Dictionary<string, List<string>>();

            if (registro.Procedimientos == null || registro.Procedimientos.Count <= 1)
                return errors;

            var ordenesDistintas = registro.Procedimientos
                .Select(p => p.NumeroOrden)
                .Distinct()
                .ToList();

            if (ordenesDistintas.Count > 1)
            {
                var ordenes = string.Join(", ", ordenesDistintas);
                AddError(errors, "procedimientos",
                    $"Se encontraron múltiples órdenes ({ordenes}) en los procedimientos recetados. Todos deben compartir la misma orden.");
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
