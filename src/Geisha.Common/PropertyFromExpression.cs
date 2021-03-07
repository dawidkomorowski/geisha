using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Geisha.Common
{
    public static class PropertyFromExpression
    {
        public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            return propertyExpression.Body switch
            {
                MemberExpression {Member: PropertyInfo propertyInfo} => propertyInfo.Name,
                _ => throw new ArgumentException("Expression must be selecting property.", nameof(propertyExpression))
            };
        }
    }
}