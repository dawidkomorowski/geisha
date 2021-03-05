using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Provides API to get <see cref="ISceneBehaviorFactory" /> instance by behavior name.
    /// </summary>
    public interface ISceneBehaviorFactoryProvider
    {
        /// <summary>
        ///     Gets <see cref="ISceneBehaviorFactory" /> instance for specified <paramref name="behaviorName" />.
        /// </summary>
        /// <param name="behaviorName">Name of <see cref="SceneBehavior" />.</param>
        /// <returns>Instance of <see cref="ISceneBehaviorFactory" /> creating behaviors of specified name.</returns>
        /// <exception cref="SceneBehaviorFactoryNotFoundException">
        ///     Thrown when no factory is available for specified
        ///     <paramref name="behaviorName" />.
        /// </exception>
        ISceneBehaviorFactory Get(string behaviorName);
    }

    internal sealed class SceneBehaviorFactoryProvider : ISceneBehaviorFactoryProvider
    {
        private readonly Dictionary<string, ISceneBehaviorFactory> _factories;

        public SceneBehaviorFactoryProvider(IEnumerable<ISceneBehaviorFactory> factories)
        {
            var factoriesArray = factories as ISceneBehaviorFactory[] ?? factories.ToArray();

            if (DuplicatesFound(factoriesArray, out var exceptionMessage))
            {
                throw new ArgumentException(exceptionMessage, nameof(factories));
            }

            _factories = factoriesArray.ToDictionary(f => f.BehaviorName);
        }

        public ISceneBehaviorFactory Get(string behaviorName)
        {
            if (_factories.TryGetValue(behaviorName, out var factory))
            {
                return factory;
            }

            throw new SceneBehaviorFactoryNotFoundException(behaviorName, _factories.Values);
        }

        private static bool DuplicatesFound(IEnumerable<ISceneBehaviorFactory> factories, out string exceptionMessage)
        {
            exceptionMessage = string.Empty;

            var groupsOfDuplicates = factories
                .GroupBy(f => f.BehaviorName)
                .Where(g => g.Count() > 1)
                .ToList();

            var duplicatesFound = groupsOfDuplicates.Any();

            if (duplicatesFound)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(
                    $"Found multiple implementations of {nameof(ISceneBehaviorFactory)} for the same behavior name. Only one factory per behavior name is allowed.");

                foreach (var duplicates in groupsOfDuplicates)
                {
                    stringBuilder.AppendLine($"Duplicates for behavior name \"{duplicates.Key}\":");

                    foreach (var factory in duplicates)
                    {
                        stringBuilder.AppendLine($"- {factory.GetType().FullName}");
                    }
                }

                exceptionMessage = stringBuilder.ToString();
            }

            return duplicatesFound;
        }
    }

    /// <summary>
    ///     The exception that is thrown when no implementation of <see cref="ISceneBehaviorFactory" /> was found for specified
    ///     behavior name.
    /// </summary>
    public sealed class SceneBehaviorFactoryNotFoundException : Exception
    {
        /// <summary>
        ///     Creates new instance of <see cref="SceneBehaviorFactoryNotFoundException" /> class.
        /// </summary>
        /// <param name="sceneBehaviorName">Name of behavior for which factory was not found.</param>
        /// <param name="sceneBehaviorFactories">Collection of all available factories.</param>
        public SceneBehaviorFactoryNotFoundException(string sceneBehaviorName, IReadOnlyCollection<ISceneBehaviorFactory> sceneBehaviorFactories) : base(
            GetMessage(sceneBehaviorName, Sorted(sceneBehaviorFactories)))
        {
            SceneBehaviorName = sceneBehaviorName;
            SceneBehaviorFactories = Sorted(sceneBehaviorFactories);
        }

        /// <summary>
        ///     Name of behavior for which factory was not found.
        /// </summary>
        public string SceneBehaviorName { get; }

        /// <summary>
        ///     Collection of all available factories.
        /// </summary>
        public IEnumerable<ISceneBehaviorFactory> SceneBehaviorFactories { get; }

        private static string GetMessage(string sceneBehaviorName, IEnumerable<ISceneBehaviorFactory> factories)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"No implementation of {nameof(ISceneBehaviorFactory)} with behavior name \"{sceneBehaviorName}\" was found.");
            stringBuilder.AppendLine("Available factories:");

            foreach (var factory in factories)
            {
                stringBuilder.AppendLine($"- {factory.GetType().FullName} for behavior name \"{factory.BehaviorName}\"");
            }

            return stringBuilder.ToString();
        }

        private static IEnumerable<ISceneBehaviorFactory> Sorted(IEnumerable<ISceneBehaviorFactory> factories)
        {
            return factories.OrderBy(f => f.BehaviorName);
        }
    }
}