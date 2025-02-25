using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace P01_2022MR655_2022AJ650.Models
{
    public class EspaciosParqueo
    {
        [Key]
        public int Id { get; set; }

        public int SucursalId { get; set; }
        [JsonIgnore]
        public Sucursales Sucursal { get; set; }


        public int Numero { get; set; }


        public string Ubicacion { get; set; }

        public decimal CostoPorHora { get; set; }

        public string Estado { get; set; }
        [JsonIgnore]
        public ICollection<Reservas>? Reservas { get; set; }

    }
}
