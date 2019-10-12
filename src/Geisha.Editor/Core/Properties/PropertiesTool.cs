using Geisha.Editor.Core.Docking;

namespace Geisha.Editor.Core.Properties
{
    internal sealed class PropertiesTool : Tool
    {
        public PropertiesTool(PropertiesViewModel viewModel) : base("Properties", new PropertiesView(), viewModel, true)
        {
        }
    }
}