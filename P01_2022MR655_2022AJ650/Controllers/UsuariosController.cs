using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022MR655_2022AJ650.Models;
using System.Security.Cryptography;
using System.Text;
namespace P01_2022MR655_2022AJ650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ParqueoContext _context;

        public UsuariosController(ParqueoContext context)
        {
            _context = context;
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateUsuario(Usuario usuario)
        {
            
            usuario.PasswordHash = ComputeSha256Hash(usuario.PasswordHash);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

      
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

    
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

           
            var usuarioExistente = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

           
            if (usuarioExistente.PasswordHash != usuario.PasswordHash)
            {
                usuario.PasswordHash = ComputeSha256Hash(usuario.PasswordHash);
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

     
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest _loginRequest)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == _loginRequest.Correo);

            if (usuario == null)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            var hashedPassword = ComputeSha256Hash(_loginRequest.Password);
            if (usuario.PasswordHash != hashedPassword)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            return Ok("Credenciales válidas.");
        }

       
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
              
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
