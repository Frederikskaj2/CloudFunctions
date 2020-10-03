using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable disable

namespace Frederikskaj2.CloudFunctions.Validation
{
    public sealed class ValidationResult<T, TError>
    {
        public ValidationResult(T value) => (Value, Errors) = (value, Enumerable.Empty<ValidationError<TError>>());

        public ValidationResult(T value, ValidationError<TError> error)
        {
            if (!error.IsError)
                throw new ArgumentException("Validation error is not an error.", nameof(error));
            (Value, Errors) = (value, new[] { error });
        }

        public ValidationResult(ValidationResult<T, TError> result, IEnumerable<ValidationError<TError>> errors) => (Value, Errors) = (result.Value, result.Errors.Concat(errors));

        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "The alternate name FromT() is unintuitive and the method also triggers CA1000.")]
        public static implicit operator ValidationResult<T, TError>(T value) => new ValidationResult<T, TError>(value);

        public T Value { get; }

        public IEnumerable<ValidationError<TError>> Errors { get; }

        public bool IsValid => !Errors.Any();

        public override string ToString()
            => IsValid ? Value.ToString() ?? string.Empty : string.Join(Environment.NewLine, Errors.Select(error => error.ToString(0)));
    }
}
