using System;
using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Application.Features.Queries.AppUser.LoginUser
{
	public class LoginUserQueryHandler : IRequestHandler<LoginUserQueryRequest, LoginUserQueryResponse>
	{
        private readonly IAuthService _authService;

        public LoginUserQueryHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginUserQueryResponse> Handle(LoginUserQueryRequest request, CancellationToken cancellationToken)
        {
            DTOs.Token token = await _authService.LoginAsync(request.UsernameOrEmail, request.Password, 15);

            return new LoginUserSuccessCommandResponse()
            {
                Token = token
            };
        }
    }
}

