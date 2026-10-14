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
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<List<ProductoDTO>>> GetProducts()
        {
            var productos = await _context.Productos
            .OrderBy(e => e.Id)
            .Select(e => new ProductoDTO{
                Id = e.Id, 
                CategoriaId = e.CategoriaId,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                Precio = e.Precio,
                Stock = e.Stock,
                ImagenUrl = e.ImagenUrl,
                CategoriaNombre = e.Categoria.Nombre
                })
            .ToListAsync();

            return Ok(productos);
        }

        // GET: api/Categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProduct(int id)
        {
            var producto = await _context.Productos
            .Where(e => e.Id == id)
                .Select(e => new ProductoDTO
                {
                    Id = e.Id,
                    CategoriaId = e.CategoriaId,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    Precio = e.Precio,
                    Stock = e.Stock,
                    ImagenUrl = e.ImagenUrl,
                    CategoriaNombre = e.Categoria.Nombre
                })
                .FirstOrDefaultAsync();
                
            if (producto == null)
                return NotFound(new { message = "Producto no encontrado." });

            return Ok(producto);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductoDTO>> UpdateProduct(int id, [FromBody] ProductoDTO dto)
        {
            var nombre = dto.Nombre?.Trim();
            if (string.IsNullOrEmpty(nombre))
                return BadRequest(new { message = "El nombre es obligatorio." });

            if (dto.Precio < 0)
                return BadRequest(new { message = "El precio debe ser mayor o igual a 0." });

            if (dto.Stock < 0)
                return BadRequest(new { message = "El stock debe ser mayor o igual a 0." });

            if (!await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId))
                return BadRequest(new { message = "La categoría no existe." });

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound(new { message = "Producto no encontrado." });

            producto.Nombre = nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Precio = dto.Precio;
            producto.Stock = dto.Stock;
            producto.CategoriaId = dto.CategoriaId;

            await _context.SaveChangesAsync();

            return await GetProduct(id);
        }
    }
}