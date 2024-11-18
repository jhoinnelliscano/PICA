using eCommerce.Commons.ExtensionMethods.DataTableExt;
using eCommerce.Products.Availability.Infraestructure.Models.UnitOfWorks;
using eCommerce.Products.Availability.Core.Contracts.Repositories;
using eCommerce.Products.Availability.Core.Objects.DbTypes;
using eCommerce.Products.Availability.Infraestructure.Contexts.DbProduct;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Products.Availability.Infraestructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbProductContext _dbcontext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory _scopeFactory;

        public ProductRepository(DbProductContext dbcontext, IUnitOfWork unitOfWork, IServiceScopeFactory scopeFactory)
        {
            _dbcontext = dbcontext;
            _unitOfWork = unitOfWork;
            _scopeFactory = scopeFactory;
        }

        public async Task<ProductEntity> GetProduct(long productId)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DbProductContext>();
                return await dbContext.SpGetProductAsync(productId);
            }
            return null;
        }

        public async Task<bool> UpdateProduct(ProductEntity productEntity)
        {
            var result = await _dbcontext.SpUpdateProductAsync(productEntity);
            await _unitOfWork.ConfirmAsync();
            return result;
        }

        public async Task<bool> RemoveUnitsToProductStock(long productId, int units)
        {
            var result = false;
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DbProductContext>();
                result = await dbContext.SpUpdateProductStockAsync(productId, units);
            }
            return result;
        }
    }
}
