using Auth10Api.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth10Api.Infrastructure.Filters;

public class ValidatorFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidatorFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {        
        var dto = context.ActionArguments.Values.FirstOrDefault();

        if (dto != null)
        {           
            var validatorType = typeof(IValidator<>).MakeGenericType(dto.GetType());

            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator != null)
            {
                var validationContext = new ValidationContext<object>(dto);

                var validationResult = await validator.ValidateAsync(validationContext);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                    context.Result = new BadRequestObjectResult(Result<object>.Failure(errors));

                    return;
                }
            }
        }

        await next();
    }
}
