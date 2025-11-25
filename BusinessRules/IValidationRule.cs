using System.Collections.Generic;
using HistoriasClinicas.Api.Models;

namespace HistoriasClinicas.Api.BusinessRules
{
   
    public interface IValidationRule
    {
        Task<Dictionary<string, List<string>>> Validar(RegistroClinico registro, RegistroValidacionContext contexto);

        string NombreRegla { get; }
    }
}
