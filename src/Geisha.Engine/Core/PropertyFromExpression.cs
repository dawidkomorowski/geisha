using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Geisha.Engine.Core
{
    /// <summary>
    ///     Utility class for inspecting property in property expression.
    /// </summary>
    public static class PropertyFromExpression
    {
        /// <summary>
        ///     Gets name of property selected in <paramref name="propertyExpression" />.
        /// </summary>
        /// <param name="propertyExpression">Expression selecting property.</param>
        /// <typeparam name="T">Type of object from which property is to be selected.</typeparam>
        /// <typeparam name="TProperty">Type of property.</typeparam>
        /// <returns>Name of selected property.</returns>
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