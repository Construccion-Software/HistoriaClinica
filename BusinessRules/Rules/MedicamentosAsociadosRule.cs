using HistoriasClinicas.Api.Models;


namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    /// <summary>
    /// Regla: En caso de haber varios medicamentos recetados, todos van asociados a la misma orden.
    public class MedicamentosAsociadosRule : IValidationRule
    {
        public string NombreRegla => "Medicamentos Asociados a Misma Orden";

        public async Task<Dictionary<string, List<string>>> Validar(HistoriaClinica historia)
        {
            var errors = new Dictionary<string, List<string>>();


            if (historia.Medicamentos == null || historia.Medicamentos.Count <= 1)
                return errors;

            var ordenesDistintas = historia.Medicamentos
                .Select(m => m.NumeroOrden)
                .Distinct()
                .ToList();

            if (ordenesDistintas.Count > 1)
            {
                var ordenes = string.Join(", ", ordenesDistintas);
                AddError(errors, "medicamentos",
                    $"Se encontraron múltiples órdenes ({ordenes}) en los medicamentos recetados. " +
                    "Todos los medicamentos recetados deben estar asociados a la misma orden.");
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
