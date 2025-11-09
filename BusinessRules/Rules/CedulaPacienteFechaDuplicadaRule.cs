using HistoriasClinicas.Api.Models;
using HistoriasClinicas.Api.Repositories;


namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    /// <summary>
    /// Regla: Valida que no exista duplicidad de historia clínica por paciente y fecha.

    public class CedulaPacienteFechaDuplicadaRule : IValidationRule
    {
        private readonly HistoriaClinicaRepository _repo;
        public string NombreRegla => "Cédula-Paciente Fecha Duplicada";

        public CedulaPacienteFechaDuplicadaRule(HistoriaClinicaRepository repo)
        {
            _repo = repo;
        }

        public async Task<Dictionary<string, List<string>>> Validar(HistoriaClinica historia)
        {
            var errors = new Dictionary<string, List<string>>();

            try
            {
                var existente = await _repo.GetByCedulaAndFechaAsync(
                    historia.CedulaPaciente,
                    historia.FechaAtencion
                );

                if (existente != null)
                {
                    AddError(errors, "cedulaPaciente",
                        $"Ya existe una historia clínica para el paciente con cédula {historia.CedulaPaciente} " +
                        $"en la fecha {historia.FechaAtencion:yyyy-MM-dd}. No se pueden crear duplicados.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en validación de duplicados: {ex.Message}");
            }

            return errors;
        }

        private void AddError(Dictionary<string, List<string>> errors, string key, string message)
        {
            if (!errors.ContainsKey(key))
                errors[key] = new List<string>();
            errors[key].Add(message);
        }
    }
}