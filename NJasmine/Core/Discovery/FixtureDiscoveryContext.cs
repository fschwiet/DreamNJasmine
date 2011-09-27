using System;
using System.Collections.Generic;
using NUnit.Core;

namespace NJasmine.Core.Discovery
{
    public class FixtureDiscoveryContext
    {
        public Func<SpecificationFixture> FixtureFactory;
        public SpecificationFixture FixtureInstanceForDiscovery;
        public NameGenerator NameGenator;

        public FixtureDiscoveryContext(Func<SpecificationFixture> fixtureFactory, NameGenerator nameGenerator, SpecificationFixture fixtureInstanceForDiscovery)
        {
            FixtureFactory = fixtureFactory;
            NameGenator = nameGenerator;
            FixtureInstanceForDiscovery = fixtureInstanceForDiscovery;
        }
    }
}