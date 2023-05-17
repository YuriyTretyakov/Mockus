using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace Mockus.Client.Extensions
{
    internal class Common
    {
        internal static string GetPropertyName(PropertyInfo propertyInfo)
        {
            var attr = propertyInfo
                .GetCustomAttributes()
                .FirstOrDefault(c => c.GetType() == typeof(JsonPropertyAttribute));

            if (attr != null)
            {
                var instance = (attr as JsonPropertyAttribute);
                return instance.PropertyName;
            }

            return propertyInfo.Name;
        }

        internal static PropertyInfo GetVerifiedPropertyInfo<TModel>(Expression<Func<TModel, object>> expression)
            where TModel : class
        {
            if (expression?.Body is not MemberExpression)
            {
                throw new ArgumentException($"Not supported for {expression}");
            }

            var propertyInfo = (expression.Body as MemberExpression)?.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new ArgumentException($"Unable to cast {nameof(expression)} to PropertyInfo");
            }

            if (typeof(ICollection<>).IsAssignableFrom(propertyInfo.PropertyType))
            {
                throw new ArgumentException($"Not supported for {propertyInfo.PropertyType}");
            }

            return propertyInfo;
        }
    }
}
    
