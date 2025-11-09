using HistoriasClinicas.Api.Models;
using HistoriasClinicas.Api.Models.Enums;


namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    /// <summary>
    /// Regla: Cuando se receta una ayuda diagnóstica PENDIENTE, no puede recetarse procedimiento ni medicamento 
    /// ya que no se tiene certeza del diagnóstico.
    /// Si la ayuda está FINALIZADA, sí se pueden recetar meds y procs.
    public class AyudaDiagnosticaExcluyentesRule : IValidationRule
    {
        public string NombreRegla => "Ayuda Diagnóstica Excluyentes";

        public async Task<Dictionary<string, List<string>>> Validar(HistoriaClinica historia)
        {
            var errors = new Dictionary<string, List<string>>();

          
            if (historia.AyudasDiagnosticas == null || !historia.AyudasDiagnosticas.Any())
                return errors;

            bool hayAyudasPendientes = historia.AyudasDiagnosticas
                .Any(a => a.Estado == EstadoAyudaDiagnostica.Pendiente || 
                          a.Estado == EstadoAyudaDiagnostica.ConResultados);

            if (hayAyudasPendientes)
            {

                if (historia.Medicamentos != null && historia.Medicamentos.Any())
                {
                    AddError(errors, "medicamentos", 
                        "No se pueden recetar medicamentos cuando hay ayudas diagnósticas pendientes de procesar. " +
                        "Espere los resultados de la ayuda diagnóstica para recetar medicamentos.");
                }


                if (historia.Procedimientos != null && historia.Procedimientos.Any())
                {
                    AddError(errors, "procedimientos",
                        "No se pueden recetar procedimientos cuando hay ayudas diagnósticas pendientes de procesar. " +
                        "Espere los resultados de la ayuda diagnóstica para recetar procedimientos.");
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
