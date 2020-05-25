using System;
using Geisha.Engine.Audio;
using Geisha.Engine.Input;
using Geisha.Engine.Rendering;

namespace Geisha.Engine
{
    public sealed class EngineBuilder
    {
        private IAudioBackend? _audioBackend;
        private IInputBackend? _inputBackend;
        private IRenderingBackend? _renderingBackend;

        public IEngine BuildForGame(IGame game)
        {
            if (game == null) throw new ArgumentNullException(nameof(game));
            if (_audioBackend == null) throw new InvalidOperationException($"Implementation of {nameof(IAudioBackend)} was not provided.");
            if (_inputBackend == null) throw new InvalidOperationException($"Implementation of {nameof(IInputBackend)} was not provided.");
            if (_renderingBackend == null) throw new InvalidOperationException($"Implementation of {nameof(IRenderingBackend)} was not provided.");

            return new Engine(_audioBackend, _inputBackend, _renderingBackend, game);
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