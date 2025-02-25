using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022MR655_2022AJ650.Dto;
using P01_2022MR655_2022AJ650.Models;
using System;

namespace P01_2022MR655_2022AJ650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EspaciosParqueoController : ControllerBase
    {
        private readonly ParqueoContext _context;

        public EspaciosParqueoController(ParqueoContext context)
        {
            _context = context;
        }

        
        [HttpGet("disponibles")]
        public async Task<IActionResult> GetEspaciosDisponibles()
        {
            var espacios = await (from e in _context.EspaciosParqueo
                                  join s in _context.Sucursales on e.SucursalId equals s.Id
                                  where e.Estado == "Disponible"
                                  select new
                                  {
                                      e.Id,
                                      e.Numero,
                                      e.Ubicacion,
                                      e.CostoPorHora,
                                      e.Estado,
                                      Sucursal = s.Nombre
                                  }).ToListAsync();

            return Ok(espacios);
        }

        
        [HttpGet("sucursal/{sucursalId}")]
        public async Task<IActionResult> GetEspaciosPorSucursal(int sucursalId)
        {
            var espacios = await (from e in _context.EspaciosParqueo
                                  where e.SucursalId == sucursalId
                                  select new
                                  {
                                      e.Id,
                                      e.Numero,
                                      e.Ubicacion,
                                      e.CostoPorHora,
                                      e.Estado
                                  }).ToListAsync();

            return Ok(espacios);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult AgregarEspacioParqueo([FromBody] EspacioParqueoDto espacioDto)
        {
            try
            {
                
                var sucursalExiste = _context.Sucursales.Any(s => s.Id == espacioDto.SucursalId);
                if (!sucursalExiste)
                    return NotFound("La sucursal especificada no existe.");

                var nuevoEspacio = new EspaciosParqueo
                {
                    SucursalId = espacioDto.SucursalId,
                    Numero = espacioDto.Numero,
                    Ubicacion = espacioDto.Ubicacion,
                    CostoPorHora = espacioDto.CostoPorHora,
                    Estado = espacioDto.Estado
                };

                _context.EspaciosParqueo.Add(nuevoEspacio);
                _context.SaveChanges();

                return Ok(nuevoEspacio);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [Route("Update/{id}")]
        public IActionResult ActualizarEspacioParqueo(int id, [FromBody] EspacioParqueoDto espacioDto)
        {
            try
            {
                // Buscar el espacio de parqueo en la base de datos
                var espacioExistente = _context.EspaciosParqueo.Find(id);
                if (espacioExistente == null)
                {
                    return NotFound("El espacio de parqueo no existe.");
                }

                // Actualizar los campos con los datos recibidos
                espacioExistente.Numero = espacioDto.Numero;
                espacioExistente.Ubicacion = espacioDto.Ubicacion;
                espacioExistente.CostoPorHora = espacioDto.CostoPorHora;
                espacioExistente.Estado = espacioDto.Estado;
                espacioExistente.SucursalId = espacioDto.SucursalId;

                _context.EspaciosParqueo.Update(espacioExistente);
                _context.SaveChanges();

                return Ok(espacioExistente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult EliminarEspacioParqueo(int id)
        {
            try
            {
                var espacio = _context.EspaciosParqueo.FirstOrDefault(e => e.Id == id);
                if (espacio == null)
                    return NotFound("El espacio de parqueo no existe.");

                _context.EspaciosParqueo.Remove(espacio);
                _context.SaveChanges();

                return Ok("Espacio de parqueo eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
