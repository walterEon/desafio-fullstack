using CatalogoApi.Data;
using CatalogoApi.DTOs;
using CatalogoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.Services
{
    public class CatalogImportService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CatalogImportService> _logger;

        public CatalogImportService(HttpClient httpClient, ApplicationDbContext context, ILogger<CatalogImportService> logger)
        {
            _httpClient = httpClient;
            _context = context;
            _logger = logger;
        }

        public async Task ImportCatalogAsync()
        {
            try
            {
                // 1. Traer las categorías y verificar si ya existen en la db
                var categoriasExternas = await _httpClient.GetFromJsonAsync<List<string>>("https://fakestoreapi.com/products/categories");
                if (categoriasExternas == null || !categoriasExternas.Any())
                {
                    _logger.LogWarning("La API externa no devolvió categorías válidas o está vacía.");
                    return;
                }
                
                var categoriasLocalesExistentes = await _context.Categorias.ToListAsync();
                foreach (var catExterna in categoriasExternas)
                {
                    bool existe = categoriasLocalesExistentes.Any(c => c.Nombre.Equals(catExterna, StringComparison.OrdinalIgnoreCase));
                    if (!existe)
                    {
                        _context.Categorias.Add(new Categoria { Nombre = catExterna });
                    }
                }
                await _context.SaveChangesAsync(); 

                // 2. Traer los productos y verificar si ya existen en la db por ExternalId
                var productosExternos = await _httpClient.GetFromJsonAsync<List<FakeStoreProductDTO>>("https://fakestoreapi.com/products");
                if (productosExternos == null || !productosExternos.Any())
                {
                    _logger.LogWarning("La API externa no devolvió productos válidos.");
                    return;
                }

                var todasLasCategorias = await _context.Categorias.ToListAsync();
                var idsExternosLocales = await _context.Productos.Select(p => p.ExternalId).ToListAsync();

                // Contadores 
                int importadosCount = 0;
                int omitidosCount = 0;

                foreach (var item in productosExternos)
                {
                    if (idsExternosLocales.Contains(item.Id))
                    {
                        omitidosCount++;
                        continue; 
                    }

                    // 3. Procesar y traducir cada producto a modelo local
                    
                    // Relacionar producto con categoría existente en la db
                    var categoriaRelacionada = todasLasCategorias.FirstOrDefault(c => 
                        c.Nombre.Equals(item.Category, StringComparison.OrdinalIgnoreCase));

                    if (categoriaRelacionada == null)
                    {
                        _logger.LogWarning($"No se encontró la categoría local para el texto: {item.Category}. Producto omitido.");
                        omitidosCount++;
                        continue;
                    }

                    var nuevoProducto = new Producto
                    {
                        ExternalId = item.Id,        
                        Nombre = item.Title,          
                        Precio = item.Price,
                        Descripcion = item.Description,
                        ImagenUrl = item.Image,       
                        Stock = 0,                    
                        CategoriaId = categoriaRelacionada.Id
                    };

                    _context.Productos.Add(nuevoProducto);
                    importadosCount++;
                }

                if (importadosCount > 0)
                {
                    await _context.SaveChangesAsync();
                }
                _logger.LogInformation($"Proceso finalizado. Productos importados con éxito: {importadosCount}. Productos omitidos (duplicados/existentes): {omitidosCount}.");
                }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al procesar el catálogo.");
                throw;
            }
        }
    }
}