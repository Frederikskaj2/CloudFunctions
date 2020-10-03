namespace Frederikskaj2.CloudFunctions.Validation
{
    public delegate ValidationError<TError> ValidationRule<T, TError>(T value);
}
