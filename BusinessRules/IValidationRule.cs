using HistoriasClinicas.Api.Models;
using System.Collections.Generic;

namespace HistoriasClinicas.Api.BusinessRules
{
   
    public interface IValidationRule
    {
        Task<Dictionary<string, List<string>>> Validar(HistoriaClinica historia);

        string NombreRegla { get; }
    }
}
