using System.ComponentModel.DataAnnotations;

namespace P01_2022MR655_2022AJ650.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string Telefono { get; set; }

        public string PasswordHash { get; set; }

        public string Rol { get; set; }

    }
}
