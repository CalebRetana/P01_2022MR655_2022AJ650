using System.ComponentModel.DataAnnotations;

namespace P01_2022MR655_2022AJ650.Models
{
    public class ReservaRequest
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int EspacioParqueoId { get; set; }
        [Required]
        public string? Fecha { get; set; }
        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public int CantidadHoras { get; set; }
    }
}
