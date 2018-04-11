using System;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Input.Components.Definition
{
    public class InputComponentDefinition : IComponentDefinition
    {
        public Guid InputMappingAssetId { get; set; }
    }
}