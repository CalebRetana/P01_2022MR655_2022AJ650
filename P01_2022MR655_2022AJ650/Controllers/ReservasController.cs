using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022MR655_2022AJ650.Models;

namespace P01_2022MR655_2022AJ650.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly ParqueoContext _context;

        public ReservasController(ParqueoContext context)
        {
            _context = context;
        }

        // POST: api/Reservas/Reservar
        [HttpPost("Reservar")]
        public async Task<IActionResult> Reservar(int usuarioId, int espacioParqueoId, DateTime fecha, string horaInicio, int cantidadHoras)
        {
            // Verificar que el usuario exista
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId);
            if (usuario == null)
            {
                return BadRequest("Error: Usuario no encontrado.");
            }

            // Verificar que el espacio de parqueo esté disponible
            var espacio = await _context.EspaciosParqueo.FirstOrDefaultAsync(e => e.Id == espacioParqueoId && e.Estado == "Disponible");
            if (espacio == null)
            {
                return BadRequest("Error: El espacio no está disponible.");
            }

            // Crear la reserva
            var nuevaReserva = new Reservas
            {
                UsuarioId = usuarioId,
                EspacioParqueoId = espacioParqueoId,
                Fecha = fecha.ToString("yyyy-MM-dd"),
                HoraInicio = horaInicio,
                CantidadHoras = cantidadHoras,
                Estado = "Activa"
            };

            _context.Reservas.Add(nuevaReserva);

            // Marcar el espacio como ocupado
            espacio.Estado = "Ocupado";

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Reserva creada exitosamente.", reservaId = nuevaReserva.Id });
        }
        // GET: api/Reservas/Activas/{usuarioId}
        [HttpGet("Activas/{usuarioId}")]
        public async Task<IActionResult> GetReservasActivas(int usuarioId)
        {
            var reservas = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.EspacioParqueo)
                .ThenInclude(e => e.Sucursal)
                .Where(r => r.UsuarioId == usuarioId && r.Estado == "Activa")
                .Select(r => new
                {
                    r.Id,
                    Usuario = new { r.Usuario.Id, r.Usuario.Nombre, r.Usuario.Correo },
                    Espacio = new
                    {
                        r.EspacioParqueo.Id,
                        r.EspacioParqueo.Numero,
                        r.EspacioParqueo.Ubicacion,
                        r.EspacioParqueo.CostoPorHora,
                        Sucursal = new { r.EspacioParqueo.Sucursal.Id, r.EspacioParqueo.Sucursal.Nombre }
                    },
                    r.Fecha,
                    r.HoraInicio,
                    r.CantidadHoras,
                    r.Estado
                })
                .ToListAsync();

            if (reservas.Count == 0)
            {
                return NotFound(new { mensaje = "No tienes reservas activas." });
            }

            return Ok(reservas);
        }
        [HttpDelete("CancelarReserva")]
        public async Task<IActionResult> CancelarReserva(int reservaId)
        {
            // Buscar la reserva por su Id
            var reserva = await _context.Reservas
                .Include(r => r.EspacioParqueo)
                .FirstOrDefaultAsync(r => r.Id == reservaId && r.Estado == "Activa");

            // Verificar si la reserva existe y está activa
            if (reserva == null)
            {
                return NotFound("Reserva no encontrada o ya cancelada.");
            }

            // Verificar si la fecha de la reserva ya pasó
            var fechaReserva = DateTime.Parse(reserva.Fecha); // Convertimos la fecha de la reserva a DateTime
            if (fechaReserva < DateTime.Now)
            {
                return BadRequest("No se puede cancelar la reserva porque ya ha pasado.");
            }

            // Marcar el espacio de parqueo como disponible nuevamente
            var espacioParqueo = reserva.EspacioParqueo;
            espacioParqueo.Estado = "Disponible";

            // Eliminar la reserva de la base de datos
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return Ok("Reserva cancelada y eliminada exitosamente.");
        }


        [HttpGet("EspaciosReservadosPorDia")]
        public async Task<IActionResult> EspaciosReservadosPorDia(DateTime fecha)
        {
            // Obtener todas las reservas de la fecha especificada
            var reservas = await _context.Reservas
                .Include(r => r.EspacioParqueo)
                .Include(r => r.EspacioParqueo.Sucursal)
                .Where(r => r.Fecha == fecha.ToString("yyyy-MM-dd") && r.Estado == "Activa")
                .ToListAsync();

            // Si no hay reservas para la fecha dada
            if (!reservas.Any())
            {
                return NotFound("No hay reservas para esta fecha.");
            }

            // Mapear la lista a una estructura más limpia para la respuesta
            var espaciosReservados = reservas.Select(r => new
            {
                r.Fecha,
                r.HoraInicio,
                r.CantidadHoras,
                r.EspacioParqueo.Numero,
                r.EspacioParqueo.Ubicacion,
                r.EspacioParqueo.Sucursal.Nombre,
                r.EspacioParqueo.Sucursal.Direccion
            }).ToList();

            return Ok(espaciosReservados);
        }
        [HttpGet("EspaciosReservadosPorFechaSucursal")]
        public async Task<IActionResult> EspaciosReservadosPorFechaSucursal(int sucursalId, DateTime fechaInicio, DateTime fechaFin)
        {
            // Obtener las reservas de la sucursal específica entre las fechas dadas
            var reservas = await _context.Reservas
                .Include(r => r.EspacioParqueo)
                .Include(r => r.EspacioParqueo.Sucursal)
                .Where(r => r.EspacioParqueo.SucursalId == sucursalId
                        && DateTime.Parse(r.Fecha) >= fechaInicio
                        && DateTime.Parse(r.Fecha) <= fechaFin
                        && r.Estado == "Activa")
                .ToListAsync();

            // Si no hay reservas para el rango de fechas
            if (!reservas.Any())
            {
                return NotFound("No hay reservas para este rango de fechas.");
            }

            // Mapear la lista a una estructura más limpia para la respuesta
            var espaciosReservados = reservas.Select(r => new
            {
                r.Fecha,
                r.HoraInicio,
                r.CantidadHoras,
                r.EspacioParqueo.Numero,
                r.EspacioParqueo.Ubicacion,
                r.EspacioParqueo.Sucursal.Nombre,
                r.EspacioParqueo.Sucursal.Direccion
            }).ToList();

            return Ok(espaciosReservados);
        }


    }
}

