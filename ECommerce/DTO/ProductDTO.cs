using E_Commerce.Date;

namespace ECommerce.DTO
{
    public class ProductDTO
    {

            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public int CategoryId { get; set; }
        
    }
    public class UpdatePriceDTO
    {
        public decimal NewPrice { get; set; }
    }
    public class UpdateStockDTO
    {
        public int NewStock { get; set; }



    }
}


