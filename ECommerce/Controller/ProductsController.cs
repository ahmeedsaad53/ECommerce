using E_Commerce.Date;
using ECommerce.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _Contex;
        public ProductsController(AppDbContext Context)
        {
            _Contex = Context;
        }

        //Get all products
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProduct()
        {
            var products = await _Contex.Products.Select(p => new ProductDTO
            {

               Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryId = p.CategoryId
            }
            ).ToListAsync();
            if (products == null || products.Count == 0)
            {
                return NotFound("No products found.");
            }
            return Ok(products);

        }



        //Get products with pagination
        [HttpGet("big-list")]
        [Authorize]
        public async Task<IActionResult> GetAllProduct(int page = 1, int pageSize = 10)
        {
            var products = await _Contex.Products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDTO
                {
                    Id=p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();

            return Ok(products);
        }




        //Get product by id
        [HttpGet("{id:int}")]
        [Authorize]

        public async Task <IActionResult> GetProduct(int id)
        {
            var product = await _Contex.Products.Where(p => p.Id == id).Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price= p.Price,
                Stock= p.Stock,
                CategoryId = p.CategoryId
            }).FirstOrDefaultAsync();
            if (product == null) return NotFound("Product not found");
     
            return Ok(product);
        }




        //Get products count
        [HttpGet("count")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetProductsCount()
        {
           var Count = await _Contex.Products.CountAsync();
            return Ok(Count);
        }


        //Get products in stock
        [HttpGet("Stock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProductsInStock()
        {
            var products = await _Contex.Products.Where(p => p.Stock > 0).Select(p => new ProductDTO
            {
                Name = p.Name,
                Description = p.Description,
                Stock = p.Stock,
                Price = p.Price,
                CategoryId = p.CategoryId
            }).ToListAsync();
            return Ok(products);
        }



        //Search products by name or description
        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchProducts([FromQuery] string keyword)
        {

            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword is required");

            var products = await _Contex.Products.
            Where(p => EF.Functions.Like(p.Name, $"%{keyword}%") || EF.Functions.Like(p.Description, $"%{keyword}%")).Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Stock = p.Stock,
                Price = p.Price,
                CategoryId = p.CategoryId
            }).ToListAsync();
            return Ok(products);

        }


        //Filter products by price range and stock availability
        [HttpGet("filter")]
        [Authorize]
        public async Task<IActionResult> FilterProducts([FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? minStock, [FromQuery] int? maxStock)
        {
            var products = await _Contex.Products.Where(p => 
                (minPrice == null || p.Price >= minPrice) &&
                (maxPrice == null || p.Price <= maxPrice) &&
                (minStock == null || p.Stock >= minStock) &&
                (maxStock == null || p.Stock <= maxStock)
            ).Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Stock = p.Stock,
                Price = p.Price,
                CategoryId = p.CategoryId
            }).ToListAsync();
            return Ok(products);
        }





        //Create new product
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateProduct(ProductDTO productDTO)
        {
            if (productDTO == null) return BadRequest("product data is required");
            if (productDTO.CategoryId <= 0) return BadRequest("CategoryId is required");
            if (!await _Contex.Categories.AnyAsync(c => c.Id == productDTO.CategoryId))
                return BadRequest("Category does not exist");

            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Stock = productDTO.Stock,
                CategoryId = productDTO.CategoryId
            };
            _Contex.Products.Add(product);
            await _Contex.SaveChangesAsync();
            return Ok(product);
        }



        //Delete product by id
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]

        public IActionResult DeleteProduct(int id)
        {
            var product = _Contex.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound("Product not found");
            _Contex.Products.Remove(product);
            _Contex.SaveChanges();
            return Ok("Product deleted successfully");
        }

        //Update product price
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin")]

        public IActionResult editPrice(int id, [FromBody] UpdatePriceDTO dto)
        {
            var product = _Contex.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound("Product not found");
            product.Price = dto.NewPrice;
            _Contex.SaveChanges();
            return Ok(product);
        }



        //Update product stock
        [HttpPatch("{id:int}/stock")]
        [Authorize(Roles = "Admin")]

        public IActionResult editStock(int id, [FromBody] UpdateStockDTO dto)
        {
            var product = _Contex.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound("Product not found");
            product.Stock = dto.NewStock;
            _Contex.SaveChanges();
            return Ok(product);
        }



        //Update product details
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]

        public IActionResult UpdateProduct(int id, ProductDTO productDTO)
        {
            var product = _Contex.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound("Product not found");
            if (productDTO.CategoryId <= 0) return BadRequest("CategoryId is required");
            if (!_Contex.Categories.Any(c => c.Id == productDTO.CategoryId))
                return BadRequest("Category does not exist");

            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Price = productDTO.Price;
            product.Stock = productDTO.Stock;
            product.CategoryId = productDTO.CategoryId;
            _Contex.SaveChanges();
            return Ok();
        }


    }
}
