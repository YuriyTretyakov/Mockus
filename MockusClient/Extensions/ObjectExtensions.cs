using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mockus.Client.Extensions
{
    public static class ObjectExtensions
    {
        public static IEnumerable<TData> Upsert<TData>(this List<TData> iEnumerable,Action<List<TData>> act, Func<List<TData>, IEnumerable<TData>> func)
        {
            var list = iEnumerable?.ToList() ?? new List<TData>();

            act.Invoke(list);


            return func.Invoke(list);
        }

        public static string GetJsonForProperty<TModel>(
           this object instance,
           Expression<Func<TModel, object>> expression,
           string formatter = "\"{0}\":\"{1}\"")
           where TModel : class
        {

            var propertyInfo = Common.GetVerifiedPropertyInfo(expression);
            var value = propertyInfo.GetValue(instance)?.ToString();
            var name = Common.GetPropertyName(propertyInfo);
            return string.Format(formatter, name, value);
        }

      


       
    }
}
