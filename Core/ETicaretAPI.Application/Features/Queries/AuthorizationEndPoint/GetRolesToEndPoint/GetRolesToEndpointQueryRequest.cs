using System;
using MediatR;

namespace ETicaretAPI.Application.Features.Queries.AuthorizationEndPoint.GetRolesToEndPoint
{
    public class GetRolesToEndpointQueryRequest : IRequest<GetRolesToEndpointQueryResponse>
    {
        public string Code { get; set; }
        public string Menu { get; set; }
    }
}

