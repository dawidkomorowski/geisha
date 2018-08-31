using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Geisha.Common.Logging;

namespace Geisha.Engine.Core
{
    public sealed class HostServices
    {
        private static readonly ILog Log = LogFactory.Create(typeof(HostServices));
        private readonly List<Action<CompositionContainer>> _servicesRegistrationActions = new List<Action<CompositionContainer>>();

        public void RegisterService<TService>(TService service)
        {
            _servicesRegistrationActions.Add(compositionContainer =>
            {
                Log.Info($"Registered host service: {typeof(TService).FullName}");
                compositionContainer.ComposeExportedValue(service);
            });
        }

        internal void RegisterServicesInContainer(CompositionContainer compositionContainer)
        {
            Log.Info("Registering host services...");

            foreach (var serviceRegistrationAction in _servicesRegistrationActions)
            {
                serviceRegistrationAction(compositionContainer);
            }

            Log.Info("Registration of host services completed.");
        }
    }
}