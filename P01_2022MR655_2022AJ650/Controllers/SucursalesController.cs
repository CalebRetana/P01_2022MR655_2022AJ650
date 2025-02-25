using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022MR655_2022AJ650.Dto;
using P01_2022MR655_2022AJ650.Models;
using System;
using System.Linq;

namespace P01_2022MR655_2022AJ650.Controllers
{
    [ApiController]
    [Route("api/sucursales")]
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
                var sucursales = _context.Sucursales
                    .Join(_context.Usuarios,
                        s => s.AdministradorId,
                        u => u.Id,
                        (s, u) => new SucursalDto
                        {
                            Nombre = s.Nombre,
                            Direccion = s.Direccion,
                            Telefono = s.Telefono,
                            Administrador = u.Nombre,
                            EspaciosDisponibles = s.NumeroEspacios
                        })
                    .ToList();

                return Ok(sucursales);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Get/{id}")]
        public IActionResult GetSucursal(int id)
        {
            try
            {
                var sucursal = _context.Sucursales
                    .Where(s => s.Id == id)
                    .Join(_context.Usuarios,
                        s => s.AdministradorId,
                        u => u.Id,
                        (s, u) => new SucursalDto
                        {
                            Nombre = s.Nombre,
                            Direccion = s.Direccion,
                            Telefono = s.Telefono,
                            Administrador = u.Nombre,
                            EspaciosDisponibles = s.NumeroEspacios
                        })
                    .FirstOrDefault();

                if (sucursal == null)
                    return NotFound("La sucursal no existe.");

                return Ok(sucursal);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult AddSucursal([FromBody] SucursalDto sucursalDto)
        {
            if (sucursalDto == null)
                return BadRequest("Los datos de la sucursal no pueden estar vacíos.");

            try
            {
                var administrador = _context.Usuarios.FirstOrDefault(u => u.Nombre == sucursalDto.Administrador);
                if (administrador == null)
                    return BadRequest("El administrador especificado no existe.");

                var nuevaSucursal = new Sucursales
                {
                    Nombre = sucursalDto.Nombre,
                    Direccion = sucursalDto.Direccion,
                    Telefono = sucursalDto.Telefono,
                    AdministradorId = administrador.Id,
                    NumeroEspacios = sucursalDto.EspaciosDisponibles
                };

                _context.Sucursales.Add(nuevaSucursal);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetSucursal), new { id = nuevaSucursal.Id }, nuevaSucursal);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear la sucursal: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // 🔹 ACTUALIZAR UNA SUCURSAL
        [HttpPut]
        [Route("Update/{id}")]
        public IActionResult ActualizarSucursal(int id, [FromBody] SucursalDto sucursalDto)
        {
            try
            {
                var sucursalExistente = _context.Sucursales.Find(id);
                if (sucursalExistente == null)
                    return NotFound("La sucursal no existe.");

                var administrador = _context.Usuarios.FirstOrDefault(u => u.Nombre == sucursalDto.Administrador);
                if (administrador == null)
                    return BadRequest("El administrador especificado no existe.");

                sucursalExistente.Nombre = sucursalDto.Nombre;
                sucursalExistente.Direccion = sucursalDto.Direccion;
                sucursalExistente.Telefono = sucursalDto.Telefono;
                sucursalExistente.AdministradorId = administrador.Id;
                sucursalExistente.NumeroEspacios = sucursalDto.EspaciosDisponibles;

                _context.Sucursales.Update(sucursalExistente);
                _context.SaveChanges();

                return Ok(sucursalExistente);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar la sucursal: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // 🔹 ELIMINAR UNA SUCURSAL
        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult EliminarSucursal(int id)
        {
            try
            {
                var sucursal = _context.Sucursales.Find(id);
                if (sucursal == null)
                    return NotFound("La sucursal no existe.");

                _context.Sucursales.Remove(sucursal);
                _context.SaveChanges();

                return Ok("Sucursal eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar la sucursal: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}
