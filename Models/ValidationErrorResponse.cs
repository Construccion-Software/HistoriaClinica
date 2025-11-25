
namespace HistoriasClinicas.Api.Services
{

    public class ValidationErrorResponse
    {

        public string Type { get; set; } = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        public string Title { get; set; } = "Errores de validaci�n";
        public int Status { get; set; } = 400;
        public string TraceId { get; set; } = string.Empty;
        public Dictionary<string, List<string>> Errors { get; set; } = new();


    }

}