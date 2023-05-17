using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mockus.Contracts.Options;
using Mockus.Contracts.Enums;
using Newtonsoft.Json;

namespace Mockus.Client.Extensions
{
    public class OptionJsonifier<TModel> where TModel: class
    {
        private readonly object _instance;
        const string DEFAULT_STRING_VALUE_PATTERN = "(?=^.*\"{0}\"(?:\\s+)?\\:(?:\\s+)?\"{1}\")";
        const string DEFAULT_NON_STRING_VALUE_PATTERN = "(?=^.*\"{0}\"(?:\\s+)?\\:(?:\\s+)?{1})";


        private readonly RegexOptions _regexOptions;
        private readonly string _overridenRegex;

        readonly List<string> _options = new();


        public OptionJsonifier(object instance)
        {
            _instance = instance;
            _regexOptions = RegexOptions.Singleline | RegexOptions.Multiline;
        }

        public OptionJsonifier(object instance, string pattern): this(instance)
        {
            _overridenRegex = pattern;
        }

        public OptionJsonifier<TModel> WithOption(Expression<Func<TModel, object>> expression)
        {
            var stringValue = JsonifyPropertyWithValue(_instance, expression);
            _options.Add(stringValue);
            return this;
        }


        public StringOption Create()
        {
            return new StringOption(string.Join(string.Empty, _options), StringCompareOptions.Pattern);
        }


        private string JsonifyPropertyWithValue(
            object instance,
            Expression<Func<TModel, object>> expression)
        {
            var propertyInfo = Common.GetVerifiedPropertyInfo(expression);
            var value = propertyInfo.GetValue(instance)?.ToString();
            var name = Common.GetPropertyName(propertyInfo);
            var formatter = GetFormatterByValueType(propertyInfo);

            var normalizedValue = NormalizeText(value);
            return string.Format(formatter, name, normalizedValue);
        }

        


        private string GetFormatterByValueType(PropertyInfo property)
        {
            if (!string.IsNullOrEmpty(_overridenRegex))
            {
                return _overridenRegex;
            }

            return property.PropertyType.IsAssignableTo(typeof(string)) ? 
                DEFAULT_STRING_VALUE_PATTERN : DEFAULT_NON_STRING_VALUE_PATTERN;
        }

        private string NormalizeText(string rawString)
        {
            return rawString.Replace("[", "\\[");
        }
    }
}
