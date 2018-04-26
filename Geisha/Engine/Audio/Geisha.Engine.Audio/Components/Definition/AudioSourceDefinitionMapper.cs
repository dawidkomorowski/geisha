using System;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Audio.Components.Definition
{
    internal sealed class AudioSourceDefinitionMapper : ComponentDefinitionMapperAdapter<AudioSource, AudioSourceDefinition>
    {
        protected override AudioSourceDefinition ToDefinition(AudioSource component)
        {
            throw new NotImplementedException();
        }

        protected override AudioSource FromDefinition(AudioSourceDefinition componentDefinition)
        {
            throw new NotImplementedException();
        }
    }
}