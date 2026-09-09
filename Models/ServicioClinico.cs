namespace GestorIAAS.Models
{
    public class ServicioClinico
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool EsEspecialidad { get; set; }

        public List<RegistroIAAS> Registros { get; set; } = new();
    }
}