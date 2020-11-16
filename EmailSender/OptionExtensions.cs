using LanguageExt;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    static class OptionExtensions
    {
        public static T? NullIfNone<T>(this Option<T> option) where T : class =>
            option.IfNoneUnsafe((T) null!);
    }
}
