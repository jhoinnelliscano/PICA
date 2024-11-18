using eCommerce.Commons.Objects.Requests.Products;
using eCommerce.Commons.Objects.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Core.Contracts.Services;
using Products.Core.Objects.Dtos;
using WebApiProducts.Config;

namespace WebApiProducts.Controllers
{
    //[AuthorizationFilter]
    [Route("api/Product/Review")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly ILogger<ProductReviewController> _logger;
        private readonly IProductServices _productService;

        public ProductReviewController(IProductServices productService, ILogger<ProductReviewController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Crea un review para un producto
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> CreateProductReview(ProductReviewRequest request)
        {
            var msgBody = new { productId = request.ProductCode, score = request.Value };
            _logger.LogInformation("Create.ProductReview.Ok {msgBody}", msgBody);
            var productReview = new ProductReviewDto(request.ProductCode, request.UserId, request.UserName, request.Review, request.Value);
            await _productService.CreateProductReview(productReview);
            return Ok(new ServiceResponse<bool>("Successful", true));
        }
    }
}
