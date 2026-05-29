using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace YoutubeSummarizer.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null)
                    continue;

                var argumentType = argument.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = context.HttpContext.RequestServices.GetService(validatorType);

                if (validator is null)
                    continue;

                var validateMethod = validatorType.GetMethod("ValidateAsync",
                    [argumentType, typeof(CancellationToken)]);

                var resultTask = (Task<FluentValidation.Results.ValidationResult>)validateMethod!
                    .Invoke(validator, [argument, CancellationToken.None])!;

                var result = await resultTask;

                if (!result.IsValid)
                {
                    context.Result = new BadRequestObjectResult(result.Errors.First().ErrorMessage);
                    return;
                }
            }

            await next();
        }
    }
}
