using System;
namespace ETicaretAPI.Application.Features.Queries.Order.GetAll
{
	public class GetAllOrdersQueryResponse
	{
		public int TotalOrderCount { get; set; }
		public object Orders { get; set; }

		//public string OrderCode { get; set; }
		//public string UserName { get; set; }
		//public float TotalPrice { get; set; }
		//public DateTime CreatedDate { get; set; }
	}
}

