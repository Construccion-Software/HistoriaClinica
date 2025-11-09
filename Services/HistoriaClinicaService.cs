using HistoriasClinicas.Api.BusinessRules;
using HistoriasClinicas.Api.Models;
using HistoriasClinicas.Api.Repositories;


namespace HistoriasClinicas.Api.Services
{
  
    public class HistoriaClinicaService
    {
        private readonly HistoriaClinicaValidator _validator;
        private readonly HistoriaClinicaRepository _repo;

        public HistoriaClinicaService(HistoriaClinicaRepository repo)
        {
            _repo = repo;
            _validator = new HistoriaClinicaValidator();
            _validator.SetRepository(repo);
        }

        public async Task<Dictionary<string, List<string>>> ValidarHistoria(HistoriaClinica historia)
        {
            var errors = new Dictionary<string, List<string>>();

        
            ValidarFormatosCedulas(historia, errors);
            ValidarLongitudes(historia, errors);

       
            if (errors.Count > 0)
                return errors;

  
            var erroresReglas = await _validator.Validar(historia);
            foreach (var kvp in erroresReglas)
            {
                if (!errors.ContainsKey(kvp.Key))
                    errors[kvp.Key] = new List<string>();
                errors[kvp.Key].AddRange(kvp.Value);
            }

            return errors;
        }

        private void ValidarFormatosCedulas(HistoriaClinica historia, Dictionary<string, List<string>> errors)
        {
            if (!string.IsNullOrWhiteSpace(historia.CedulaMedico))
            {
                if (historia.CedulaMedico.Length > 10 || !historia.CedulaMedico.All(char.IsDigit))
                {
                    AddError(errors, "cedulaMedico", 
                        "La cédula del médico debe contener maximo 10 dígitos numéricos.");
                }
            }

            if (!string.IsNullOrWhiteSpace(historia.CedulaPaciente))
            {
                if (historia.CedulaPaciente.Length > 10 || !historia.CedulaPaciente.All(char.IsDigit))
                {
                    AddError(errors, "cedulaPaciente", 
                        "La cédula del paciente debe contener exactamente 10 dígitos numéricos.");
                }
            }
        }

        /// <summary>
        /// Valida longitudes máximas de campos de texto.
        /// </summary>
        private void ValidarLongitudes(HistoriaClinica historia, Dictionary<string, List<string>> errors)
        {
            const int MAX_MOTIVO = 500;
            const int MAX_SINTOMATOLOGIA = 1000;
            const int MAX_DIAGNOSTICO = 1000;

            if (!string.IsNullOrWhiteSpace(historia.MotivoConsulta) && historia.MotivoConsulta.Length > MAX_MOTIVO)
                AddError(errors, "motivoConsulta", 
                    $"El motivo de consulta no puede exceder {MAX_MOTIVO} caracteres.");

            if (!string.IsNullOrWhiteSpace(historia.Sintomatologia) && historia.Sintomatologia.Length > MAX_SINTOMATOLOGIA)
                AddError(errors, "sintomatologia", 
                    $"La sintomatología no puede exceder {MAX_SINTOMATOLOGIA} caracteres.");

            if (!string.IsNullOrWhiteSpace(historia.Diagnostico) && historia.Diagnostico.Length > MAX_DIAGNOSTICO)
                AddError(errors, "diagnostico", 
                    $"El diagnóstico no puede exceder {MAX_DIAGNOSTICO} caracteres.");

            if (historia.Medicamentos != null)
            {
                for (int i = 0; i < historia.Medicamentos.Count; i++)
                {
                    var med = historia.Medicamentos[i];
                    if (med.NumeroOrden > 999999)
                        AddError(errors, $"medicamentos[{i}].numeroOrden", 
                            "Número de orden no puede exceder 999999 (6 dígitos).");
                }
            }

            if (historia.Procedimientos != null)
            {
                for (int i = 0; i < historia.Procedimientos.Count; i++)
                {
                    var proc = historia.Procedimientos[i];
                    if (proc.NumeroOrden > 999999)
                        AddError(errors, $"procedimientos[{i}].numeroOrden", 
                            "Número de orden no puede exceder 999999 (6 dígitos).");
                    if (proc.RequiereAsistenciaEspecialista && (proc.Especialidades == null || !proc.Especialidades.Any()))
                        AddError(errors, $"procedimientos[{i}].especialidades", 
                            "Es obligatorio añadir especialidades en procedimientos que requieren asistencia.");
                }
            }

            if (historia.AyudasDiagnosticas != null)
            {
                for (int i = 0; i < historia.AyudasDiagnosticas.Count; i++)
                {
                    var ayuda = historia.AyudasDiagnosticas[i];
                    if (ayuda.NumeroOrden > 999999)
                        AddError(errors, $"ayudasDiagnosticas[{i}].numeroOrden", 
                            "Número de orden no puede exceder 999999 (6 dígitos).");
                    if (ayuda.RequiereAsistenciaEspecialista && (ayuda.Especialidades == null || !ayuda.Especialidades.Any()))
                        AddError(errors, $"ayudasDiagnosticas[{i}].especialidades", 
                            "Es obligatorio añadir especialidades en ayudas diagnósticas que requieren asistencia.");
                }
            }
        }

        private void AddError(Dictionary<string, List<string>> errors, string key, string message)
        {
            if (!errors.ContainsKey(key))
                errors[key] = new List<string>();

            errors[key].Add(message);
        }
    }
}
