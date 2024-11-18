using eCommerce.ShoppingCart.Core.Objects.DbTypes;

namespace eCommerce.ShoppingCart.Core.Contracts.Repositories
{
    public interface IShoppingCartWriteRepository
    {
        Task<bool> CreateShoppingCart(IEnumerable<ShoppingCartEntity> shoppingCartEntities);
        Task<bool> UpdateShoppingCart(ShoppingCartEntity shoppingCartEntity);
        Task<bool> UpdateShoppingCartStatus(long porductId, bool status);
        Task<bool> DeleteShoppingCartItem(ShoppingCartEntity shoppingCartEntity);
        Task<bool> DeleteShoppingCartByUser(string userId, long porductId);
    }
}
