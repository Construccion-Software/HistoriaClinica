using HistoriasClinicas.Api.Models;


namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    

public class CamposObligatoriosRule: IValidationRule
    {

        public string NombreRegla => "Campos Obligatorios";
        public async Task<Dictionary<string, List<string>>> Validar(HistoriaClinica historia)
        {
            var errors = new Dictionary<string, List<string>>();

            //Valida cedula del paciente
            if (string.IsNullOrWhiteSpace(historia.CedulaPaciente))
                AddError(errors, "cedulaPaciente", "La cédula del paciente es obligatoria");

            //Valida cedula del Medico
            if (string.IsNullOrWhiteSpace(historia.CedulaMedico))
                AddError(errors, "cedulaMedico", "La cédula del Medico es obligatoria");

            return await Task.FromResult(errors);
        }
        

        public void AddError(Dictionary<string, List<string>> errors, string key, string message)
        {
            if (!errors.ContainsKey(key)) errors[key] = new List<string>();
            errors[key].Add(message);
        }
        
    }

}