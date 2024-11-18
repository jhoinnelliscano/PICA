using eCommerce.Commons.Objects.Messaging;
using eCommerce.Products.Availability.Core.Config;
using eCommerce.Products.Availability.Core.Contracts.Repositories;
using eCommerce.Products.Availability.Core.Contracts.Services;
using eCommerce.Products.Availability.Core.Objects.DbTypes;
using eCommerce.Products.Availability.Core.Objects.Dtos;
using eCommerce.PublisherSubscriber.Contracts;
//using eCommerce.PublisherSubscriber.MessagingRabbit;
using eCommerce.PublisherSubscriber.Object;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eCommerce.Products.Availability.Infraestructure.Services
{
    public class ProductService : IProductService
    {
        private IProductRepository _productRepository;
        private readonly IPublisher _publisherMessage;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, IPublisher publisherMessage, IConfiguration configuration
            , ILogger<ProductService>  logger)
        {
            _productRepository = productRepository;
            _publisherMessage = publisherMessage;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> UpdateProduct(ProductDto product)
        {
            var productActuality = await _productRepository.GetProduct(product.Id);
            var result = await _productRepository.UpdateProduct(new ProductEntity(product.Id, product.Price, product.Stock));

            _logger.LogInformation($"Se actualiza el producto id: { product.Id }");

            if (result && productActuality.Price != product.Price)
            {
                _logger.LogInformation($"Se actualiza precio del producto id: { product.Id}, precio anterior: { productActuality.Price }, precio nuevo: { product.Price } ");

                var notifyObject = new ProductPriceUpdatedMsg(product.Id, product.Price);
                var mqMessage = new Message<ProductPriceUpdatedMsg>("Updated product price", notifyObject);
                var message = JsonConvert.SerializeObject(mqMessage);
                _publisherMessage.PublishMessage(message, _configuration.GetValue<string>("AppConfiguration:MQ:ProductPriceUpdatedQueue"));
            }

            if (product.Stock <= 0)
            {
                _logger.LogInformation($"Sin stock producto id: { product.Id}");
                await NotifyProductStockChange(product.Id, false);
            }

            if (productActuality.Stock <= 0 && product.Stock > 0)
            {
                _logger.LogInformation($"Disponible producto id: { product.Id}");
                await NotifyProductStockChange(product.Id, true);
            }

            return result;
        }

        public async Task<bool> UdateProductStock(long productId, int units)
        {
            var result = await _productRepository.RemoveUnitsToProductStock(productId, units);
            var product = await _productRepository.GetProduct(productId);

            if (result && product.Stock <= 0)
            {
                _logger.LogInformation($"Sin stock producto id: { productId }");
                await NotifyProductStockChange(productId, false);
            }
            return result;
        }

        private Task NotifyProductStockChange(long productId, bool available) 
        {

            var notifyObject = new ProductUpdateStockMsg(productId, available);
            var mqMessage = new Message<ProductUpdateStockMsg>("Updated product stock", notifyObject);
            var message = JsonConvert.SerializeObject(mqMessage);
            _publisherMessage.PublishMessage(message, _configuration.GetValue<string>("AppConfiguration:MQ:ProductStockUpdateQueue"));
            return Task.CompletedTask;
        }
    }
}
