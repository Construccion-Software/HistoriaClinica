using HistoriasClinicas.Api.Models;


namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    /// <summary>
    /// Regla: En caso de haber varios procedimientos solicitados, todos van asociados a la misma orden.

    public class ProcedimientosAsociadosRule : IValidationRule
    {
        public string NombreRegla => "Procedimientos Asociados a Misma Orden";

        public async Task<Dictionary<string, List<string>>> Validar(HistoriaClinica historia)
        {
            var errors = new Dictionary<string, List<string>>();

    
            if (historia.Procedimientos == null || historia.Procedimientos.Count <= 1)
                return errors;


            var ordenesDistintas = historia.Procedimientos
                .Select(p => p.NumeroOrden)
                .Distinct()
                .ToList();

            if (ordenesDistintas.Count > 1)
            {
                var ordenes = string.Join(", ", ordenesDistintas);
                AddError(errors, "procedimientos",
                    $"Se encontraron múltiples órdenes ({ordenes}) en los procedimientos recetados. " +
                    "Todos los procedimientos recetados deben estar asociados a la misma orden.");
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
