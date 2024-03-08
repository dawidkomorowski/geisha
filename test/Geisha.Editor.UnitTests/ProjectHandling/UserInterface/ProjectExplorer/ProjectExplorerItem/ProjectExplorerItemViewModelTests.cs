using System.Collections.Generic;
using System.Linq;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem
{
    [TestFixture]
    public class ProjectExplorerItemViewModelTests
    {
        [TestCaseSource(nameof(UpdateItemsTestCases))]
        public void UpdateItems_ShouldSetItemsAsExpected(UpdateItemsTestCase testCase)
        {
            // Arrange
            var vm = new ProjectExplorerItemViewModelImpl("Some name");

            var initialItems = testCase.InitialItems.Select(i => new ProjectExplorerItemViewModelImpl(i)).ToList();
            var expectedItems = testCase.ExpectedItems.Select(i => new ProjectExplorerItemViewModelImpl(i));

            foreach (var item in initialItems)
            {
                vm.Items.Add(item);
            }

            // Assume
            Assert.That(vm.Items.Select(i => i.Name), Is.EqualTo(testCase.InitialItems));

            // Act
            vm.PublicUpdateItems(expectedItems);

            // Assert
            Assert.That(vm.Items.Select(i => i.Name), Is.EqualTo(testCase.AssertExpectedItems));
        }

        private static IEnumerable<UpdateItemsTestCase> UpdateItemsTestCases => new List<UpdateItemsTestCase>
        {
            new UpdateItemsTestCase
            {
                Name = "Should insert expected items when initial items are empty.",
                InitialItems = Enumerable.Empty<string>(),
                ExpectedItems = new[] {"Item1", "Item2", "Item3"},
                AssertExpectedItems = new[] {"Item1", "Item2", "Item3"}
            },
            new UpdateItemsTestCase
            {
                Name = "Should remove all items when expected items are empty.",
                InitialItems = new[] {"Item1", "Item2", "Item3"},
                ExpectedItems = Enumerable.Empty<string>(),
                AssertExpectedItems = Enumerable.Empty<string>()
            },
            new UpdateItemsTestCase
            {
                Name = "Should remove and add some items to make Items as in expected items.",
                InitialItems = new[] {"Item1", "Item2", "Item3"},
                ExpectedItems = new[] {"Item2", "Item3", "Item4"},
                AssertExpectedItems = new[] {"Item2", "Item3", "Item4"}
            },
            new UpdateItemsTestCase
            {
                Name = "Should reorder items to make Items as in expected items.",
                InitialItems = new[] {"Item1", "Item2", "Item3"},
                ExpectedItems = new[] {"Item3", "Item2", "Item1"},
                AssertExpectedItems = new[] {"Item3", "Item2", "Item1"}
            }
        };

        public class UpdateItemsTestCase
        {
            public string Name { get; set; } = string.Empty;
            public IEnumerable<string> InitialItems { get; set; } = Enumerable.Empty<string>();
            public IEnumerable<string> ExpectedItems { get; set; } = Enumerable.Empty<string>();
            public IEnumerable<string> AssertExpectedItems { get; set; } = Enumerable.Empty<string>();

            public override string ToString() => Name;
        }

        private class ProjectExplorerItemViewModelImpl : ProjectExplorerItemViewModel
        {
            public ProjectExplorerItemViewModelImpl(string name) : base(name)
            {
            }

            public void PublicUpdateItems(IEnumerable<ProjectExplorerItemViewModel> expectedItems)
            {
                UpdateItems(expectedItems);
            }
        }
    }
}