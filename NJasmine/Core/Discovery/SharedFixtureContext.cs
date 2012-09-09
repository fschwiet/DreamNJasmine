using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core.Discovery
{
    public class SharedFixtureContext<T> : FixtureContext where T : SharedFixture
    {
        public T InstanceForDiscovery;

        public SharedFixtureContext(FixtureContext containingContext, Func<T> creator) 
            : base(containingContext.NativeTestFactory, containingContext.FixtureFactory, containingContext.NameReservations, containingContext.GlobalSetupOwner, containingContext.GlobalSetupManager)
        {
            GlobalSetupManager = containingContext.GlobalSetupOwner.CreateSetupManager(typeof (T), () => creator());
        }
    }
}
