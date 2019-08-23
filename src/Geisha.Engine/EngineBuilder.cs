using System;
using Geisha.Common.Extensibility;
using Geisha.Engine.Audio;
using Geisha.Engine.Input;

namespace Geisha.Engine
{
    public sealed class EngineBuilder
    {
        private IAudioBackend _audioBackend;
        private IInputBackend _inputBackend;
        private IHostServices _hostServices;

        public IEngine Build()
        {
            return new Engine(_audioBackend, _inputBackend, _hostServices);
        }

        public EngineBuilder UseAudioBackend(IAudioBackend audioBackend)
        {
            _audioBackend = audioBackend;
            return this;
        }

        public EngineBuilder UseInputBackend(IInputBackend inputBackend)
        {
            _inputBackend = inputBackend;
            return this;
        }

        [Obsolete]
        public EngineBuilder UseHostServices(IHostServices hostServices)
        {
            _hostServices = hostServices;
            return this;
        }
    }
}