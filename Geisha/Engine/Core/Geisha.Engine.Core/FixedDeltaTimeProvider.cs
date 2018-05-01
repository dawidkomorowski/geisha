﻿using System.ComponentModel.Composition;
using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Core
{
    public interface IFixedDeltaTimeProvider
    {
        double GetFixedDeltaTime();
    }

    [Export(typeof(IFixedDeltaTimeProvider))]
    internal class FixedDeltaTimeProvider : IFixedDeltaTimeProvider
    {
        private readonly IConfigurationManager _configurationManager;

        [ImportingConstructor]
        public FixedDeltaTimeProvider(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public double GetFixedDeltaTime()
        {
            return _configurationManager.GetConfiguration<CoreConfiguration>().FixedDeltaTime;
        }
    }
}