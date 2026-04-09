using System;
using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Repositories.IProductImageFileRepositories;
using ETicaretAPI.Application.Repositories.IProductRepositories;
using MediatR;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage
{
	public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommandRequest
												,UploadProductImageCommandResponse>
	{
        private readonly IProductReadRepository _productReadRepository;
        private readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        private readonly IStorageService _storageService;

        public UploadProductImageCommandHandler(IProductReadRepository productReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IStorageService storageService)
        {
            _productReadRepository = productReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _storageService = storageService;
        }

        public async Task<UploadProductImageCommandResponse> Handle(UploadProductImageCommandRequest request, CancellationToken cancellationToken)
        {
            List<(string fileName, string pathOrContainerName)> result
    = await _storageService.UploadAsync("photo-images", request.Files);

            ETicaretAPI.Domain.Entities.Product product = await _productReadRepository.GetByIdAsync(request.Id);


            await _productImageFileWriteRepository.AddRangeAsync(result.Select(q => new ETicaretAPI.Domain.Entities.ProductImageFile
            {
                FileName = q.fileName,
                Path = q.pathOrContainerName,
                Storage = _storageService.StorageName,
                Products = new List<ETicaretAPI.Domain.Entities.Product>() { product }
            }).ToList());

            await _productImageFileWriteRepository.SaveAsync();
            return new();
        }
    }
}

