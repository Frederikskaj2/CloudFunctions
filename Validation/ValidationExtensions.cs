namespace Frederikskaj2.CloudFunctions.Validation
{
    public static class ValidationExtensions
    {
        public static ComposableValidationRule<T, TError> ToComposable<T, TError>(this ValidationRule<T, TError> rule)
            => result
                =>
                {
                    var error = rule(result.Value);
                    return !error.IsError ? new ValidationResult<T, TError>(result.Value) : new ValidationResult<T, TError>(result.Value, error);
                };

        public static ComposableValidationRule<T, TError> And<T, TError>(this ComposableValidationRule<T, TError> rule1, ValidationRule<T, TError> rule2)
            => result
                =>
                {
                    var innerResult = rule1(result);
                    return innerResult.IsValid ? rule2.ToComposable()(innerResult) : innerResult;
                };

        public static ComposableValidationRule<T, TError> And<T, TError>(this ComposableValidationRule<T, TError> rule1, ComposableValidationRule<T, TError> rule2)
            => result
                =>
                {
                    var innerResult = rule1(result);
                    return innerResult.IsValid ? rule2(innerResult) : innerResult;
                };

        public static ComposableValidationRule<T, TError> Or<T, TError>(this ComposableValidationRule<T, TError> rule1, ValidationRule<T, TError> rule2)
            => result
                =>
                {
                    var innerResult = rule1(result);
                    var nextResult = rule2.ToComposable()(innerResult);
                    return new ValidationResult<T, TError>(innerResult, nextResult.Errors);
                };

        public static ComposableValidationRule<T, TError> Or<T, TError>(this ComposableValidationRule<T, TError> rule1, ComposableValidationRule<T, TError> rule2)
            => result
                =>
                {
                    var innerResult = rule1(result);
                    var nextResult = rule2(innerResult);
                    return new ValidationResult<T, TError>(innerResult, nextResult.Errors);
                };
    }
}
