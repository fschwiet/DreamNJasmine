using System;

namespace NJasmine.Core.Discovery
{
    internal class FixtureDiscoveryContext
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