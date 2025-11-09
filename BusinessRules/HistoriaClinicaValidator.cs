using HistoriasClinicas.Api.BusinessRules.Rules;
using HistoriasClinicas.Api.Models;
using HistoriasClinicas.Api.Repositories;



namespace HistoriasClinicas.Api.BusinessRules
{

    public class HistoriaClinicaValidator
    {
        private readonly List<IValidationRule> _validationRules;
        private HistoriaClinicaRepository? _repo;

        public HistoriaClinicaValidator()
        {
       
            _validationRules = new List<IValidationRule>
            {
                new CamposObligatoriosRule(),
                new AyudaDiagnosticaExcluyentesRule(),
                new OrdenesUnicasRule(),
                new MedicamentosAsociadosRule(),
                new ProcedimientosAsociadosRule(),
                new ItemsUnicosEnOrdenRule()
            };
        }

        public void SetRepository(HistoriaClinicaRepository repo)
        {
            _repo = repo;
            if (_repo != null && !_validationRules.Any(r => r.NombreRegla == "Cédula-Paciente Fecha Duplicada"))
            {
                _validationRules.Add(new CedulaPacienteFechaDuplicadaRule(_repo));
            }
        }

        public async Task<Dictionary<string, List<string>>> Validar(HistoriaClinica historia)
        {
            var erroresConsolidados = new Dictionary<string, List<string>>();

            foreach (var rule in _validationRules)
            {
                var erroresRegla = await rule.Validar(historia);
                foreach (var kvp in erroresRegla)
                {
                    if (!erroresConsolidados.ContainsKey(kvp.Key))
                        erroresConsolidados[kvp.Key] = new List<string>();

                    erroresConsolidados[kvp.Key].AddRange(kvp.Value);
                }
            }

            return erroresConsolidados;
        }

        public async Task<Dictionary<string, List<string>>> ValidarReglaEspecifica(HistoriaClinica historia, string nombreRegla)
        {
            var rule = _validationRules.FirstOrDefault(r => r.NombreRegla == nombreRegla);
            if (rule == null)
                return new Dictionary<string, List<string>>();
            
            return await rule.Validar(historia);
        }
        public List<string> ObtenerNombresReglas()
        {
            return _validationRules.Select(r => r.NombreRegla).ToList();
        }
    }
}
