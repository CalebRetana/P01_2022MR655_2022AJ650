using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022MR655_2022AJ650.Dto;
using P01_2022MR655_2022AJ650.Models;

namespace P01_2022MR655_2022AJ650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SucursalesController : ControllerBase
    {
        private readonly ParqueoContext _context;

        public SucursalesController(ParqueoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetSucursales()
        {
            try
            {
                var sucursales = (from s in _context.Sucursales
                                  join u in _context.Usuarios on s.AdministradorId equals u.Id
                                  select new SucursalDto
                                  {
                                       
                                      Nombre = s.Nombre,
                                      Direccion = s.Direccion,
                                      Telefono = s.Telefono,
                                      Administrador = u.Nombre,
                                      EspaciosDisponibles = s.NumeroEspacios
                                  }).ToList();

                return Ok(sucursales);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Get/{id}")]
        public IActionResult GetSucursal(int id)
        {
            try
            {
                var sucursal = (from s in _context.Sucursales
                                join u in _context.Usuarios on s.AdministradorId equals u.Id
                                where s.Id == id
                                select new SucursalConEspaciosDto
                                {
                                
                                    Nombre = s.Nombre,
                                    Direccion = s.Direccion,
                                    Telefono = s.Telefono,
                                    Administrador = u.Nombre,
                                    EspaciosDisponibles = s.NumeroEspacios,
                                    Espacios = (from e in _context.EspaciosParqueo
                                                where e.SucursalId == s.Id
                                                select new EspacioParqueoDto
                                                {
                                                    
                                                    Numero = e.Numero,
                                                    Ubicacion = e.Ubicacion,
                                                    CostoPorHora = e.CostoPorHora,
                                                    Estado = e.Estado
                                                }).ToList()
                                }).FirstOrDefault();

                if (sucursal == null)
                    return NotFound("La sucursal no existe.");

                return Ok(sucursal);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
