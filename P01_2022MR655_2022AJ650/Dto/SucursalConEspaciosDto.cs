namespace P01_2022MR655_2022AJ650.Dto
{
    public class SucursalConEspaciosDto : SucursalDto
    {
        public List<EspacioParqueoDto> Espacios { get; set; } = new List<EspacioParqueoDto>();
    }
}
