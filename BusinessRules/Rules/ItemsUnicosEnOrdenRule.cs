using HistoriasClinicas.Api.Models;


namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    /// <summary>
    /// Regla: No puede existir dos elementos dentro de la misma orden que correspondan al mismo ítem, 
    /// a pesar de que uno corresponda a un medicamento y el otro a un procedimiento.

    public class ItemsUnicosEnOrdenRule : IValidationRule
    {
        public string NombreRegla => "Items Únicos en Orden";

        public async Task<Dictionary<string, List<string>>> Validar(HistoriaClinica historia)
        {
            var errors = new Dictionary<string, List<string>>();

            var medicamentos = historia.Medicamentos ?? new List<MedicamentoRecetado>();
            var procedimientos = historia.Procedimientos ?? new List<ProcedimientoRealizado>();

            if (!medicamentos.Any() || !procedimientos.Any())
                return errors;


            var ordenesMedicamentos = medicamentos.GroupBy(m => m.NumeroOrden).ToDictionary(g => g.Key, g => g.ToList());
            var ordenesProcedimientos = procedimientos.GroupBy(p => p.NumeroOrden).ToDictionary(g => g.Key, g => g.ToList());


            var ordenesComunes = ordenesMedicamentos.Keys.Intersect(ordenesProcedimientos.Keys).ToList();

            foreach (var orden in ordenesComunes)
            {
                var itemsMedicamentos = ordenesMedicamentos[orden].Select(m => m.Item).ToHashSet();
                var itemsProcedimientos = ordenesProcedimientos[orden].Select(p => p.Item).ToHashSet();


                var itemsDuplicados = itemsMedicamentos.Intersect(itemsProcedimientos).ToList();

                foreach (var item in itemsDuplicados)
                {
                    AddError(errors, "ordenes",
                        $"El ítem {item} aparece tanto en medicamentos como en procedimientos dentro de la orden {orden}. " +
                        "Un ítem no puede estar asignado a ambos tipos de prescripciones en la misma orden.");
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
