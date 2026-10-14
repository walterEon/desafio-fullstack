namespace CatalogoApi.DTOs
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; } = 0;
        public string ImagenUrl { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
    }
}