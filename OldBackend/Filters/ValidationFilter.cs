using System.ComponentModel.DataAnnotations;

namespace BugTrackerBackend.Filters;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        T? argument = context.GetArgument<T>(0);
        if (argument is not null)
        {
            List<ValidationResult>? validationResults = [];
            if (!Validator.TryValidateObject(argument, new ValidationContext(argument), validationResults, true))
            {
                Dictionary<string, string[]> validationErrors = [];
                foreach (ValidationResult v in validationResults)
                {
                    if (v.MemberNames.Any() && v.ErrorMessage is not null)
                    {
                        string field = v.MemberNames.First();
                        validationErrors.Add(field, [v.ErrorMessage]);
                    }
                }
                return Results.ValidationProblem(validationErrors);
            }
        }
        return await next.Invoke(context);
    }
}
