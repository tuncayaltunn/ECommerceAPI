using System;
using ETicaretAPI.Application.Repositories.ICustomerRepositories;
using ETicaretAPI.Application.Repositories.IOrderRepositories;
using ETicaretAPI.Application.Repositories.IProductRepositories;
using ETicaretAPI.Persistence.Repositories;
using ETicaretAPI.Persistence.Repositories.CustomerRepositories;
using ETicaretAPI.Persistence.Repositories.OrderRepositories;
using ETicaretAPI.Persistence.Repositories.ProductRepositories;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ETicaretAPI.Application.Repositories.IFileRepositories;
using ETicaretAPI.Persistence.Repositories.FileRepositories;
using ETicaretAPI.Application.Repositories.IInvoiceFileRepositories;
using ETicaretAPI.Persistence.Repositories.InvoiceFileRepositories;
using ETicaretAPI.Application.Repositories.IProductImageFileRepositories;
using ETicaretAPI.Persistence.Repositories.ProductImageFileRepositories;
using ETicaretAPI.Domain.Entities.Identity;
using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Persistence.Services;
using ETicaretAPI.Application.Abstractions.Services.Authentication;
using ETicaretAPI.Persistence.Repositories.BasketRepositories;
using ETicaretAPI.Application.Repositories.IBasketRepositories;
using ETicaretAPI.Application.Repositories.IBasketItemRepositories;
using ETicaretAPI.Persistence.Repositories.BasketItemRepositories;
using Microsoft.AspNetCore.Identity;
using ETicaretAPI.Application.Repositories.ICompletedOrderRepositories;
using ETicaretAPI.Persistence.Repositories.CompletedOrderRepositories;
using ETicaretAPI.Application.Repositories.IEndPointRepositories;
using ETicaretAPI.Application.Repositories.IMenuRepositories;
using ETicaretAPI.Persistence.Repositories.EndpointRepositories;
using ETicaretAPI.Persistence.Repositories.MenuRepositories;

namespace ETicaretAPI.Persistence
{
	public static class ServiceRegistration
	{
		public static void AddPersistenceServices(this IServiceCollection services)
		{
			services.AddDbContext<ETicaretAPIDbContext>(options =>
				options.UseNpgsql(Configuration.ConnectionString)
			);

			services.AddIdentity<AppUser, AppRole>(options =>
			{
				options.Password.RequiredLength = 8;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.User.RequireUniqueEmail = true;
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
			}).AddEntityFrameworkStores<ETicaretAPIDbContext>()
									.AddDefaultTokenProviders();

			services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();

			services.AddScoped<IOrderReadRepository, OrderReadRepository>();
            services.AddScoped<IOrderWriteRepository, OrderWriteRepository>();

            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();

			services.AddScoped<IFileReadRepository, FileReadRepository>();
            services.AddScoped<IFileWriteRepository, FileWriteRepository>();

            services.AddScoped<IInvoiceFileReadRepository, InvoiceFileReadRepository>();
            services.AddScoped<IInvoiceFileWriteRepository, InvoiceFileWriteRepository>();

            services.AddScoped<IProductImageFileReadRepository, ProductImageFileReadRepository>();
            services.AddScoped<IProductImageFileWriteRepository, ProductImageFileWriteRepository>();

			services.AddScoped<IBasketReadRepository, BasketReadRepository>();
            services.AddScoped<IBasketWriteRepository, BasketWriteRepository>();

            services.AddScoped<IBasketItemReadRepository, BasketItemReadRepository>();
            services.AddScoped<IBasketItemWriteRepository, BasketItemWriteRepository>();

            services.AddScoped<ICompletedOrderReadRepository, CompletedOrderReadRepository>();
            services.AddScoped<ICompletedOrderWriteRepository, CompletedOrderWriteRepository>();

			services.AddScoped<IEndpointReadRepository, EndpointReadRepository>();
            services.AddScoped<IEndpointWriteRepository, EndpointWriteRepository>();

			services.AddScoped<IMenuReadRepository, MenuReadRepository>();
            services.AddScoped<IMenuWriteRepository, MenuWriteRepository>();

            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IUserService, UserService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IExternalAuthentication,AuthService>();
            services.AddScoped<IInternalAuthentication, AuthService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IRoleService, RoleService>();
			services.AddScoped<IAuthorizationEndpointService, AuthorizationEndpointService>();
        } 
	}
}

