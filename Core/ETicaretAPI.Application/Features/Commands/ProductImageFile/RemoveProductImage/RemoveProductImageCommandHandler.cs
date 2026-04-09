using ETicaretAPI.Application.Repositories.IProductRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.RemoveProductImage
{
	public class RemoveProductImageCommandHandler : IRequestHandler<RemoveProductImageCommandRequest
                                                ,RemoveProductImageCommandResponse>
	{
        private readonly IProductReadRepository _productReadRepository;
        private readonly IProductWriteRepository _productWriteRepository;

        public RemoveProductImageCommandHandler(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
        }

        public async Task<RemoveProductImageCommandResponse> Handle(RemoveProductImageCommandRequest request, CancellationToken cancellationToken)
        {

            ETicaretAPI.Domain.Entities.Product? product = await _productReadRepository.Table
                            .Include(q => q.ProductImageFiles)
                            .FirstOrDefaultAsync(q => q.Id == Guid.Parse(request.Id));
            ETicaretAPI.Domain.Entities.ProductImageFile? productImageFile = product?.ProductImageFiles
                                        .FirstOrDefault(q => q.Id == Guid.Parse(request.ImageId));

            if(productImageFile != null)
                product?.ProductImageFiles.Remove(productImageFile);

            await _productWriteRepository.SaveAsync();

            return new();
        }
    }
}

