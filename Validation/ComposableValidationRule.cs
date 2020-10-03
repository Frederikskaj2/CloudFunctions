namespace Frederikskaj2.CloudFunctions.Validation
{
    public delegate ValidationResult<T, TError> ComposableValidationRule<T, TError>(ValidationResult<T, TError> result);
}
