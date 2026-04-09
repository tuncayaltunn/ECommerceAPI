using System;
using ETicaretAPI.Application.Repositories.IProductRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Application.Features.Queries.ProductImageFile.GetProductmages
{
	public class GetProductImagesQueryHandler : IRequestHandler<GetProductImagesQueryRequest
                                                ,List<GetProductImagesQueryResponse>>
	{
        private readonly IProductReadRepository _productReadRepository;

        public GetProductImagesQueryHandler(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<List<GetProductImagesQueryResponse>> Handle(GetProductImagesQueryRequest request, CancellationToken cancellationToken)
        {
            ETicaretAPI.Domain.Entities.Product? product = await _productReadRepository.Table
                .Include(q => q.ProductImageFiles)
                .FirstOrDefaultAsync(q => q.Id == Guid.Parse(request.Id));

            return product.ProductImageFiles.Select(q => new GetProductImagesQueryResponse
            {
                Path = q.Path, // Ben local storage kullandım. Path'i ona göre düzenleyebiliriz
                FileName = q.FileName,
                Id = q.Id
            }).ToList();
        }
    }
}

