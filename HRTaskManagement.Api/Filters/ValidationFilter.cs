using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRTaskManagement.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null)
                    continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator is null)
                    continue; // Bu tip için validator yazılmamış, atla

                var validationContext = new ValidationContext<object>(argument);
                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                {
                    // Mevcut ExceptionHandlingMiddleware'iniz bunu zaten 400 + errors[] olarak yakalıyor
                    throw new FluentValidation.ValidationException(result.Errors);
                }
            }

            await next();
        }
    }
}