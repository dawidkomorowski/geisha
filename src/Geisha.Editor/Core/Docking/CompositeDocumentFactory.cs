using System.Collections.Generic;
using System.Linq;

namespace Geisha.Editor.Core.Docking
{
    internal interface ICompositeDocumentFactory
    {
        Document CreateDocument(string filePath);
    }

    internal sealed class CompositeDocumentFactory : ICompositeDocumentFactory
    {
        private readonly IEnumerable<IDocumentFactory> _documentFactories;

        public CompositeDocumentFactory(IEnumerable<IDocumentFactory> documentFactories)
        {
            _documentFactories = documentFactories;
        }

        public Document CreateDocument(string filePath)
        {
            return _documentFactories.Single(f => f.IsApplicableForFile(filePath)).CreateDocument(filePath);
        }
    }
}