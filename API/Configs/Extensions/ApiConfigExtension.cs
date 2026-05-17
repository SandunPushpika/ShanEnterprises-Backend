namespace ShanEnterprises.Configs.Extensions;

public static class ApiConfigExtension
{
    public static void ConfigureControllers(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(error => new
                    {
                        Field = x.Key,
                        Error = error.ErrorMessage
                    }))
                    .ToList();

                throw new FluentValidation.ValidationException(
                    errors.Select(x =>
                        new FluentValidation.Results.ValidationFailure(
                            x.Field,
                            x.Error))
                );
            };
        });
    }
}