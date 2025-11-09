using HistoriasClinicas.Api.Models;


namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    /// <summary>
    /// Regla: Las órdenes deben ser únicas. No se repite un identificador de orden, 
    /// aunque corresponda de forma diferente a un medicamento o procedimiento.

    public class OrdenesUnicasRule : IValidationRule
    {
        public string NombreRegla => "Órdenes Únicas";

        public async Task<Dictionary<string, List<string>>> Validar(HistoriaClinica historia)
        {
            var errors = new Dictionary<string, List<string>>();

            if (historia.Medicamentos != null && historia.Medicamentos.Any())
            {
                var ordenesRepe = historia.Medicamentos
                    .GroupBy(m => m.NumeroOrden)
                    .Where(g => g.Count() > 1)
                    .ToList();

                foreach (var grupo in ordenesRepe)
                {
                    AddError(errors, "medicamentos",
                        $"El número de orden {grupo.Key} está repetido en {grupo.Count()} medicamentos. " +
                        "Las órdenes de medicamentos deben ser únicas.");
                }
            }
            if (historia.Procedimientos != null && historia.Procedimientos.Any())
            {
                var ordenesRepe = historia.Procedimientos
                    .GroupBy(p => p.NumeroOrden)
                    .Where(g => g.Count() > 1)
                    .ToList();

                foreach (var grupo in ordenesRepe)
                {
                    AddError(errors, "procedimientos",
                        $"El número de orden {grupo.Key} está repetido en {grupo.Count()} procedimientos. " +
                        "Las órdenes de procedimientos deben ser únicas.");
                }
            }

            if (historia.AyudasDiagnosticas != null && historia.AyudasDiagnosticas.Any())
            {
                var ordenesRepe = historia.AyudasDiagnosticas
                    .GroupBy(a => a.NumeroOrden)
                    .Where(g => g.Count() > 1)
                    .ToList();

                foreach (var grupo in ordenesRepe)
                {
                    AddError(errors, "ayudasDiagnosticas",
                        $"El número de orden {grupo.Key} está repetido en {grupo.Count()} ayudas diagnósticas. " +
                        "Las órdenes de ayudas diagnósticas deben ser únicas.");
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
