using Mockus.Contracts.Abstractions.Options;

namespace Mockus.Client.Extensions
{
    public static class OptionsExtensions
    {

        public static TType Get<TType>(this IOption option) where TType : IOption
        {
            return (TType) option;
        }

        public static OptionJsonifier<TModel> Build<TModel>(this object instance)
            where TModel : class
        {
            return new OptionJsonifier<TModel>(instance);
        }

        public static OptionJsonifier<TModel> Build<TModel>(this object instance, string pattern)
            where TModel : class
        {
            return new OptionJsonifier<TModel>(instance, pattern);
        }
    }
}
