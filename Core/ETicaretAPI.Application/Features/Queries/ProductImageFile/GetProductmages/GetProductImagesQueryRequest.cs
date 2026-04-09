using System;
using MediatR;

namespace ETicaretAPI.Application.Features.Queries.ProductImageFile.GetProductmages
{
	public class GetProductImagesQueryRequest : IRequest<List<GetProductImagesQueryResponse>>
	{
		public string Id { get; set; }
	}
}

