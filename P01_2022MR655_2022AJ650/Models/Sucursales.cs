using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace P01_2022MR655_2022AJ650.Models
{
    public class Sucursales
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public int AdministradorId { get; set; }

        public Usuario Administrador { get; set; }

        public int NumeroEspacios { get; set; }


        [JsonIgnore]
        public ICollection<EspaciosParqueo>? EspacioParqueo { get; set; }
    }
}
