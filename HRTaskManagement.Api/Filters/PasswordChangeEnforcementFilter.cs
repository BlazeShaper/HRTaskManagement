using HRTaskManagement.Api.Attributes;
using HRTaskManagement.Api.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRTaskManagement.Api.Filters
{
    public class PasswordChangeEnforcementFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var mustChange = user.FindFirst("MustChangePassword")?.Value;

                if (mustChange == "true")
                {
                    var endpoint = context.HttpContext.GetEndpoint();
                    var isExempt = endpoint?.Metadata.GetMetadata<AllowPasswordChangeAttribute>() != null;

                    if (!isExempt)
                    {
                        context.Result = new ObjectResult(new ErrorResponse
                        {
                            Message = "Şifrenizi değiştirmeniz gerekiyor. Lütfen /api/auth/change-password endpoint'ini kullanın."
                        })
                        {
                            StatusCode = StatusCodes.Status403Forbidden
                        };
                        return;
                    }
                }
            }

            await next();
        }
    }
}