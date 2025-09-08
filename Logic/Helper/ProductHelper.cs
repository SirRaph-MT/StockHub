using Core.Db;
using Core.DTOs;
using Core.Model;
using Logic.IHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Logic.Helper
{
    public class ProductHelper:IProductHelper
    {
        private readonly AppDbContext _context;
        public ProductHelper(AppDbContext context)
        {
         _context = context;   
        }

        public async Task<int> CreateProduct(ProductDTO dto)
        {
            var product = new Product
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }


        public async Task<List<ProductDTO>> GetProduct()
        {
            return await _context.Products
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                })
                .ToListAsync();
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<bool> EditAsync(Product product)
        {
            if (product == null)
                return false;

            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct == null)
                return false;

            _context.Entry(existingProduct).CurrentValues.SetValues(product);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false; 
            }
            catch (DbUpdateException ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<Order> PlaceOrderAsync(PlaceOrderRequest request)
        {
            if (request?.Items == null || !request.Items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order();
                decimal totalAmount = 0;

                foreach (var item in request.Items)
                {
                    var product = await _context.Products.AsTracking().FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product == null)
                        throw new ArgumentException($"Product with ID {item.ProductId} not found.");
                    if (product.StockQuantity < item.Quantity)
                        throw new ArgumentException($"Insufficient stock for Product ID {item.ProductId}. Available: {product.StockQuantity}, Requested: {item.Quantity}");

                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };
                    order.OrderItems.Add(orderItem);
                    product.StockQuantity -= item.Quantity;
                    totalAmount += item.Quantity * product.Price;
                }

                order.TotalAmount = totalAmount;
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); 
                await transaction.CommitAsync();
                return order;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new ArgumentException("Order failed due to concurrent update. Please try again.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
