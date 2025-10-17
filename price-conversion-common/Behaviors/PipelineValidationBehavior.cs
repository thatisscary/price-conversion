namespace price_conversion_common.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;

    public class ValidatorPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest: notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidatorPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
          => _validators = validators;

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
           return Handle(request, next, cancellationToken);
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Invoke the validators
            var failures = _validators
                .Select(validator => validator.Validate(request))
                .SelectMany(result => result.Errors)
                .ToArray();

            if (failures.Length > 0)
            {
                // Map the validation failures and throw an error,
                // this stops the execution of the request

                throw new ValidationException(failures);
            }

            // Invoke the next handler
            // (can be another pipeline behavior or the request handler)
            return next();
        }
    }
}