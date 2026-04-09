using System;
using ETicaretAPI.Application.DTOs;

namespace ETicaretAPI.Application.Features.Queries.AppUser.LoginUser
{
	public class LoginUserQueryResponse
	{

	}

	public class LoginUserSuccessCommandResponse : LoginUserQueryResponse
    {
        public Token Token { get; set; }
    }

	public class LoginUserErrorCommandResponse : LoginUserQueryResponse
    {
		public string Message { get; set; }
	}
}

