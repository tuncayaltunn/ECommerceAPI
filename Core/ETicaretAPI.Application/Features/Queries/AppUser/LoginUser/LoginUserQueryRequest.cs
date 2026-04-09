using System;
using MediatR;

namespace ETicaretAPI.Application.Features.Queries.AppUser.LoginUser
{
	public class LoginUserQueryRequest : IRequest<LoginUserQueryResponse>
	{
		public string UsernameOrEmail { get; set; }
		public string Password { get; set; }
	}
}

