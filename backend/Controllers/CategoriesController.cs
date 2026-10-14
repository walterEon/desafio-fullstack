using CatalogoApi.Data;
using CatalogoApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<List<CategoriaDTO>>> GetCategories()
        {
            var categorias = await _context.Categorias
            .Select(e => new CategoriaDTO{ Id = e.Id, Nombre = e.Nombre })
            .ToListAsync();

            return Ok(categorias);
        }
    }
}