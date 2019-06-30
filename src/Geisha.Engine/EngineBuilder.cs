﻿using System;
using Geisha.Common.Extensibility;
using Geisha.Engine.Core;
using Geisha.Engine.Input;

namespace Geisha.Engine
{
    public sealed class EngineBuilder
    {
        private IInputBackend _inputBackend;
        private IHostServices _hostServices;

        public IEngine Build()
        {
            return new Core.Engine(_hostServices);
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