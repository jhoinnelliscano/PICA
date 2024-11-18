using eCommerce.Commons.ExtensionMethods.DataTableExt;
using eCommerce.ShoppingCart.Core.Contracts.Repositories;
using eCommerce.ShoppingCart.Core.Objects.DbTypes;
using eCommerce.ShoppingCart.Infraestructure.Contexts;
using eCommerce.ShoppingCart.Infraestructure.Contexts.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.ShoppingCart.Infraestructure.Repositories
{
    public class ShoppingCartWriteRepository : IShoppingCartWriteRepository
    {
        private readonly DbShoppingCartWriteContext _dbcontext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartWriteRepository(DbShoppingCartWriteContext dbcontext, IUnitOfWork unitOfWork, IServiceScopeFactory scopeFactory)
        {
            _dbcontext = dbcontext;
            _unitOfWork = unitOfWork;
            _scopeFactory = scopeFactory;
        }

        public async Task<bool> CreateShoppingCart(IEnumerable<ShoppingCartEntity> shoppingCartEntities)
        {
            var dataTable = shoppingCartEntities.CopyToDataTable();
            var result = await _dbcontext.SpCreateShoppingCartAsync(dataTable);
            await _unitOfWork.ConfirmAsync();
            return result;
        }

        public async Task<bool> UpdateShoppingCart(ShoppingCartEntity shoppingCartEntity)
        {
            var result = await _dbcontext.SpUpdateShoppingCartAsync(shoppingCartEntity);
            await _unitOfWork.ConfirmAsync();
            return result;
        }

        public async Task<bool> UpdateShoppingCartStatus(long porductId, bool status)
        {
            var result = false;
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DbShoppingCartWriteContext>();
                result = await _dbcontext.SpUpdateShoppingCartStatusAsync(porductId, status);
            }
            return result;
        }

        public async Task<bool> DeleteShoppingCartItem(ShoppingCartEntity shoppingCartEntity)
        {
            var result = await _dbcontext.SpDeleteShoppingCartItemAsync(shoppingCartEntity.CustomerId, shoppingCartEntity.ProductId);
            await _unitOfWork.ConfirmAsync();
            return result;
        }

        public async Task<bool> DeleteShoppingCartByUser(string userId, long productId)
        {

            var result = false;
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DbShoppingCartWriteContext>();

                if (productId == 0)
                {
                    result = await dbContext.SpDeleteShoppingCartByUserAsync(userId);
                }
                else 
                {
                    result = await dbContext.SpDeleteShoppingCartItemAsync(userId, productId);
                }
            }
            return result;
        }
    }
}
