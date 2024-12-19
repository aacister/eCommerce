using AutoMapper;
using FluentValidation;
using ProductsService.Business.DTO;
using ProductsService.Business.ServiceContracts;
using ProductsService.DataAccess.Entities;
using ProductsService.DataAccess.RepositoryContracts;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace ProductsService.Business.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IValidator<ProductAddRequest> _productAddRequestValidator;
        private readonly IValidator<ProductUpdateRequest> _productUpdateRequestValidator;
        private readonly IMapper _mapper;
        private readonly IProductsRepository _productsRepository;

        public ProductsService(
            IValidator<ProductAddRequest> productAddRequestValidator,
            IValidator<ProductUpdateRequest> productUpdateRequestValidator,
            IMapper mapper,
            IProductsRepository productsRepository
            )
        {
            _productAddRequestValidator = productAddRequestValidator;
            _productUpdateRequestValidator = productUpdateRequestValidator;
            _mapper = mapper;
            _productsRepository = productsRepository;
        }

        public async Task<ProductResponse?> AddProduct(ProductAddRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var validationResult = await _productAddRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }

            Product productInput = _mapper.Map<Product>(request);
            Product? addedProduct = await _productsRepository.AddProduct(productInput);

            if (addedProduct == null) return null;

            ProductResponse addedProductResponse = _mapper.Map<ProductResponse>(addedProduct);
            return addedProductResponse;
        }

        public async Task<bool> DeleteProduct(Guid productID)
        {
            Product? existingProduct = await _productsRepository.GetProductByCondition(temp => temp.ProductID == productID);
            if (existingProduct == null) return false;
            bool isDeleted = await _productsRepository.DeleteProduct(productID);
            return isDeleted;
        }

        public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            Product? product = await _productsRepository.GetProductByCondition(conditionExpression);
            if (product == null) return null;
            ProductResponse productResponse = _mapper.Map<ProductResponse>(product);
            return productResponse;
        }

        public async Task<List<ProductResponse?>> GetProducts()
        {
            IEnumerable<Product?> products = await _productsRepository.GetProducts();
            IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products);
            return productResponses.ToList();
        }

        public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            IEnumerable<Product?> products = await _productsRepository.GetProductsByCondition(conditionExpression);
    
            IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products);
            return productResponses.ToList();
        }

        public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest request)
        {
            Product? existingProduct = await _productsRepository.GetProductByCondition(temp => temp.ProductID == request.ProductID);
            if (existingProduct == null) throw new ArgumentException("Invalid Product ID.");

            var validationResult = await _productUpdateRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }

            Product product = _mapper.Map<Product>(request);
            Product? updatedProduct = await _productsRepository.UpdateProduct(product);
            ProductResponse? updatedProductResponse = _mapper.Map<ProductResponse>(updatedProduct); 
            return updatedProductResponse;
        }
    }
}
