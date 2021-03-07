using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Geisha.Common;

namespace Geisha.Engine.Core.SceneModel
{
    internal static class MultipleImplementationsValidator
    {
        public static bool ShouldThrow<T, TDuplicateKey>(IEnumerable<T> implementations, Expression<Func<T, TDuplicateKey>> duplicateKeySelector,
            out string exceptionMessage) where T : notnull
        {
            exceptionMessage = string.Empty;

            var groupsOfDuplicates = implementations
                .GroupBy(duplicateKeySelector.Compile())
                .Where(g => g.Count() > 1)
                .ToList();

            var duplicatesFound = groupsOfDuplicates.Any();

            if (duplicatesFound)
            {
                var stringBuilder = new StringBuilder();
                var propertyName = PropertyFromExpression.GetPropertyName(duplicateKeySelector);

                stringBuilder.AppendLine(
                    $"Found multiple implementations of {typeof(T).FullName} for the same value of {propertyName}. Only one implementation per {propertyName} is allowed.");

                foreach (var duplicates in groupsOfDuplicates)
                {
                    stringBuilder.AppendLine($"Duplicates for {propertyName} \"{duplicates.Key}\":");

                    foreach (var implementation in duplicates)
                    {
                        stringBuilder.AppendLine($"- {implementation.GetType().FullName}");
                    }
                }

                exceptionMessage = stringBuilder.ToString();
            }

            return duplicatesFound;
        }
    }
}