namespace HistoriasClinicas.Api.Models
{

    public class ProcedimientoRealizado
    {

        public int NumeroOrden { get; set; }
        public string? IdProcedimiento { get; set; }
        public int Cantidad { get; set; }
        public bool RequiereAsistenciaEspecialista { get; set; }
        public List<Especialista>? Especialidades { get; set; }
        public int Item { get; set; }
    }


}