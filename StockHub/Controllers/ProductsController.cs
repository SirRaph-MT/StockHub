using Core.Db;
using Core.DTOs;
using Core.Model;
using Logic.IHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StockHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IProductHelper _productHelper;
        public ProductsController(AppDbContext context, IProductHelper productHelper)
        {
            _context = context;
            _productHelper = productHelper;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDTO dto)
        {
            var ProductId = await _productHelper.CreateProduct(dto);
            return Ok(ProductId);
        }


        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productHelper.GetProduct();
            return Ok(products);
        }


        [HttpGet("GetProductToEdit")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productHelper.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDTO productDto)
        {
            if (productDto == null || id != productDto.Id)
                return BadRequest("Invalid product ID or data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new Product
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity
            };

            var updated = await _productHelper.EditAsync(product);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid product ID.");

            var deleted = await _productHelper.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPost("PlaceOrder")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var order = await _productHelper.PlaceOrderAsync(request);
                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                };
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the order.");
            }
        }

        [HttpGet("GetOrder/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
                return NotFound();

            var orderDto = new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
            return Ok(orderDto);
        }
    }
}
