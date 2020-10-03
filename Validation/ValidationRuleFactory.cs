using System;
using System.Collections.Generic;
using System.Linq;

namespace Frederikskaj2.CloudFunctions.Validation
{
    public static class ValidationRuleFactory
    {
        public static ValidationRule<T, TError> Rule<T, TError>(Predicate<T> invalidPredicate, TError error)
            => value
                => invalidPredicate(value) ? error : ValidationError<TError>.NoError;

        public static ValidationRule<T, TError> ChildRule<T, TChild, TError>(Func<T, ValidationResult<TChild, TError>> validator, TError error)
            => value
                =>
                {
                    var result = validator(value);
                    return !result.IsValid ? new ValidationError<TError>(error, result.Errors) : ValidationError<TError>.NoError;
                };

        public static ValidationRule<T, TError> ChildRule<T, TChild, TError>(Predicate<T> invokePredicate, Func<T, ValidationResult<TChild, TError>> validator, TError error)
            => value
                => invokePredicate(value) ? ChildRule(validator, error)(value) : ValidationError<TError>.NoError;

        public static ValidationRule<T, TError> ChildCollectionRule<T, TChild, TError>(Func<T, IEnumerable<TChild>?> childrenSelector, Func<TChild, ValidationResult<TChild, TError>> validator, TError error, Func<int, TError> childError)
            => value
                =>
                {
                    var children = childrenSelector(value) ?? Enumerable.Empty<TChild>();
                    var errors = children
                        .Select((child, index) => (Result: validator(child), Index: index))
                        .Where(tuple => !tuple.Result.IsValid)
                        .Select(tuple => new ValidationError<TError>(childError(tuple.Index), tuple.Result.Errors))
                        .ToList();
                    if (errors.Count == 0)
                        return ValidationError<TError>.NoError;
                    return new ValidationError<TError>(error, errors);
                };
    }
}
