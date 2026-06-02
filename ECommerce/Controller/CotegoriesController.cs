using E_Commerce.Date;
using ECommerce.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var categories = _context.Categories.ToList();
            if(categories==null)return NotFound();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(CategoryDTO category)
        {
            if (category == null) return BadRequest();

            var categoryEntity = new Category
            {
                Name = category.Name
            };

            _context.Categories.Add(categoryEntity);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = categoryEntity.Id }, categoryEntity);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Put(int id, CategoryDTO category)
        {
            if (category == null) return BadRequest();

            var existingCategory = _context.Categories.Find(id);
            if (existingCategory == null) return NotFound();

            existingCategory.Name = category.Name;
            _context.SaveChanges();

            return Ok(existingCategory);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var existingCategory = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (existingCategory == null) return NotFound();

            _context.Categories.Remove(existingCategory);
            _context.SaveChanges();

            return Ok();
        }
    }
}

