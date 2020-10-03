using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable disable

namespace Frederikskaj2.CloudFunctions.Validation
{
    public sealed class ValidationError<TError>
    {
        const int indentSize = 4;

        internal static readonly ValidationError<TError> NoError = new ValidationError<TError>();

        ValidationError() { }

        public TError Error { get; }

        public IEnumerable<ValidationError<TError>> Children { get; }

        public ValidationError(TError error) => (Error, Children) = (error, Enumerable.Empty<ValidationError<TError>>());

        public ValidationError(TError error, IEnumerable<ValidationError<TError>> children) => (Error, Children) = (error, children);

        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "The alternate name FromTError() is unintuitive and the method also triggers CA1000.")]
        public static implicit operator ValidationError<TError>(TError error) => new ValidationError<TError>(error);

        public bool IsError => !Equals(Error, default);

        public string ToString(int level)
        {
            var indent = new string(' ', level * indentSize);
            var children = string.Join(Environment.NewLine, Children.Select(child => child.ToString(level + 1)));
            return $"{indent}{Error}{(children.Length > 0 ? Environment.NewLine : string.Empty)}{children}";
        }
    }
}
