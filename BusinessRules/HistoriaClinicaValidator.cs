using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HistoriasClinicas.Api.BusinessRules.Rules;
using HistoriasClinicas.Api.Models;

namespace HistoriasClinicas.Api.BusinessRules
{
    public class HistoriaClinicaValidator
    {
        private readonly List<IValidationRule> _validationRules;

        public HistoriaClinicaValidator()
        {
            _validationRules = new List<IValidationRule>
            {
                new CamposObligatoriosRule(),
                new AyudaDiagnosticaExcluyentesRule(),
                new OrdenesUnicasRule(),
                new MedicamentosAsociadosRule(),
                new ProcedimientosAsociadosRule(),
                new ItemsUnicosEnOrdenRule(),
                new CedulaPacienteFechaDuplicadaRule()
            };
        }

        public async Task<Dictionary<string, List<string>>> Validar(RegistroClinico registro, RegistroValidacionContext contexto)
        {
            var erroresConsolidados = new Dictionary<string, List<string>>();

            foreach (var rule in _validationRules)
            {
                var erroresRegla = await rule.Validar(registro, contexto);
                foreach (var kvp in erroresRegla)
                {
                    if (!erroresConsolidados.ContainsKey(kvp.Key))
                        erroresConsolidados[kvp.Key] = new List<string>();

                    erroresConsolidados[kvp.Key].AddRange(kvp.Value);
                }
            }

            return erroresConsolidados;
        }

        public async Task<Dictionary<string, List<string>>> ValidarReglaEspecifica(RegistroClinico registro, RegistroValidacionContext contexto, string nombreRegla)
        {
            var rule = _validationRules.FirstOrDefault(r => r.NombreRegla == nombreRegla);
            if (rule == null)
                return new Dictionary<string, List<string>>();

            return await rule.Validar(registro, contexto);
        }

        public List<string> ObtenerNombresReglas()
        {
            return _validationRules.Select(r => r.NombreRegla).ToList();
        }
    }
}
