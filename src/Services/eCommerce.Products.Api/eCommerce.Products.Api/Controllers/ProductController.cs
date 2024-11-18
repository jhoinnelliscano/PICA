using eCommerce.Commons.Objects.Requests.Products;
using eCommerce.Commons.Objects.Responses;
using eCommerce.Commons.Objects.Responses.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Core.Contracts.Services;
using Products.Core.Helpers.Mappers;
using Products.Core.Objects.Dtos;

namespace WebApiAuthorizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductServices _productService;
        private readonly IMapperHelper _mapperHelper;

        public ProductController(IProductServices productService, IMapperHelper mapper, ILogger<ProductController> logger)
        {
            _productService = productService;
            _mapperHelper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Consulta de catálogo de productos
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Catalog")]
        [Authorize]
        public ActionResult<ServiceResponse<ProductCatalogResponse>> GetProductCatalog([FromQuery] ProductCatalogRequest request)
        {
            _logger.LogInformation($"Get.ProductCatalog.Ok");

            var categoryId = request.CategoryId == null ? 0 : Convert.ToInt32(request.CategoryId);

            var businessObject = new ProductSearchDto(categoryId, request.ProviderId, request.Search, 
                request.Page, request.ItemsByPage, request.Sort, request.MinPrice, request.MaxPrice, request.Condition);
            var result =  _productService.GetProductCatalog(businessObject);
            var products = _mapperHelper.MappToProductResponse(result.Products);
            var response = new ProductCatalogResponse(result.ProductsFound, result.Count, products, result.Page, result.Sort, result.TotalProducts, result.TotalPages);
            return Ok(new ServiceResponse<ProductCatalogResponse>("Successful", response));
        }

        /// <summary>
        /// Consulta de un producto por su Id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Detail")]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<ProductResponse>>> GetProduct([FromQuery] ProductRequest request)
        {
            var msgBody = new { productId = request.ProductCode };
            _logger.LogInformation("Get.Product.Ok {msgBody}", msgBody);

            var result = await _productService.GetProduct(request.ProductCode);
            var response = _mapperHelper.MappToProductResponse(result);
            return Ok(new ServiceResponse<ProductResponse>("Successful", response));
        }

        /// <summary>
        /// Consulta una lista de productos por sus Ids
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List")]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<IEnumerable<ProductResponse>>>> GetProducts([FromQuery] ProductsRequest request)
        {
            var result = _productService.GetProducts(request.ProductsCode);
            var response = _mapperHelper.MappToProductResponse(result);
            return Ok(new ServiceResponse<IList<ProductResponse>>("Successful", response.ToList()));
        }
    }
}
