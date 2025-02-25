using System.ComponentModel.DataAnnotations;

namespace P01_2022MR655_2022AJ650.Models
{
    public class Reservas
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int EspacioParqueoId { get; set; }
        public EspaciosParqueo EspacioParqueo { get; set; }
        public string Fecha { get; set; }
        public string? HoraInicio { get; set; }
        public int CantidadHoras { get; set; }
        public string Estado { get; set; }

    }
}
