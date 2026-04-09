using System;
using MediatR;

namespace ETicaretAPI.Application.Features.Queries.Order.GetAll
{
	public class GetAllOrdersQueryRequest : IRequest<GetAllOrdersQueryResponse>
	{
		public int Page { get; set; } = 0;
		public int Size { get; set; } = 5;
    }
}

