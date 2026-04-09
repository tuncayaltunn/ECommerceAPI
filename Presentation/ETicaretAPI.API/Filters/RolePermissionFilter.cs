using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace ETicaretAPI.API.Filters
{
    public class RolePermissionFilter : IAsyncActionFilter
    {
        readonly IUserService _userService;

        public RolePermissionFilter(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var name = context.HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(name))
            {
                await next();
                return;
            }

            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var attribute = descriptor?.MethodInfo.GetCustomAttribute<AuthorizeDefinitionAttribute>();
            if (attribute == null)
            {
                // Korumasız endpoint; yetki kontrolü yapılmaz.
                await next();
                return;
            }

            var httpAttribute = descriptor!.MethodInfo.GetCustomAttribute<HttpMethodAttribute>();
            var httpMethod = httpAttribute?.HttpMethods.FirstOrDefault() ?? HttpMethods.Get;
            var code = $"{httpMethod}.{attribute.ActionType}.{attribute.Definition.Replace(" ", string.Empty)}";

            var hasRole = await _userService.HasRolePermissionToEndpointAsync(name, code);

            if (!hasRole)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}

