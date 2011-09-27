using System;
using System.Collections.Generic;
using NUnit.Core;

namespace NJasmine.Core.Discovery
{
    public class FixtureDiscoveryContext
    {
        public readonly Func<SpecificationFixture> FixtureFactory;
        public readonly SpecificationFixture FixtureInstanceForDiscovery;
        public readonly NameGenerator NameGenator;
        public readonly IGlobalSetupManager GlobalSetup;

        public FixtureDiscoveryContext(
            Func<SpecificationFixture> fixtureFactory, 
            NameGenerator nameGenerator, 
            IGlobalSetupManager globalSetup,
            SpecificationFixture fixtureInstanceForDiscovery)
        {
            FixtureFactory = fixtureFactory;
            NameGenator = nameGenerator;
            GlobalSetup = globalSetup;
            FixtureInstanceForDiscovery = fixtureInstanceForDiscovery;
        }
    }
}