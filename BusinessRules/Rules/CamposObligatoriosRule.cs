using System.Collections.Generic;
using HistoriasClinicas.Api.Models;

namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    public class CamposObligatoriosRule : IValidationRule
    {
        public string NombreRegla => "Campos Obligatorios";

        public async Task<Dictionary<string, List<string>>> Validar(RegistroClinico registro, RegistroValidacionContext contexto)
        {
            var errors = new Dictionary<string, List<string>>();

            if (string.IsNullOrWhiteSpace(contexto.CedulaPaciente))
                AddError(errors, "cedulaPaciente", "La cédula del paciente es obligatoria");

            if (string.IsNullOrWhiteSpace(registro.CedulaMedico))
                AddError(errors, "cedulaMedico", "La cédula del médico es obligatoria");

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