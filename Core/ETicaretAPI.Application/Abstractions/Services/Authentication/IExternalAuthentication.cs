using System;
namespace ETicaretAPI.Application.Abstractions.Services.Authentication
{
	public interface IExternalAuthentication
	{
		Task GoogleLoginAsync();
		Task FacebookLoginAsync();
	}
}

