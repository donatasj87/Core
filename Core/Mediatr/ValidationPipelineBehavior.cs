using FluentValidation;
using MediatR;

namespace Donatas.Core.Mediatr
{
    public sealed class ValidationPipelineBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(next);

            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators.Select(x => x.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(vr => vr.Errors).Where(failure => failure != null);

            if (failures.Any())
                throw new ValidationException($"Validation Failure [{typeof(TRequest).Name}]", failures);

            return await next();
        }
    }
}
