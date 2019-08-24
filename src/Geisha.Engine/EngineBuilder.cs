using Geisha.Engine.Audio;
using Geisha.Engine.Input;
using Geisha.Engine.Rendering;

namespace Geisha.Engine
{
    public sealed class EngineBuilder
    {
        private IAudioBackend _audioBackend;
        private IInputBackend _inputBackend;
        private IRenderingBackend _renderingBackend;

        public IEngine Build()
        {
            return new Engine(_audioBackend, _inputBackend, _renderingBackend);
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

        public EngineBuilder UseRenderingBackend(IRenderingBackend renderingBackend)
        {
            _renderingBackend = renderingBackend;
            return this;
        }
    }
}