namespace GestorIAAS.Models
{
    public class TipoIAAS
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool NotificaMinsal { get; set; }

        public List<RegistroIAAS> Registros { get; set; } = new();

        public string NombreMostrado => NotificaMinsal ? $"{Nombre} (Se notifica)" : Nombre;
    }
}