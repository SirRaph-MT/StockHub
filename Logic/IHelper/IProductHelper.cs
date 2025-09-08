using Core.DTOs;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelper
{
    public interface IProductHelper
    {
        Task<Product?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);

        Task<int> CreateProduct(ProductDTO dto);
        Task<List<ProductDTO>> GetProduct();
        Task<bool> EditAsync(Product product);
        Task<Order> PlaceOrderAsync(PlaceOrderRequest request);
    }
}
