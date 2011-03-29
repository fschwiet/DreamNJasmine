using System;

namespace NJasmine.Core.Discovery
{
    internal class AllSuitesBuildContext
    {
        public Func<SpecificationFixture> _fixtureFactory;
        public SpecificationFixture _fixtureInstanceForDiscovery;
        public NameGenerator _nameGenator;

        public AllSuitesBuildContext(Func<SpecificationFixture> fixtureFactory, NameGenerator nameGenerator, SpecificationFixture fixtureInstanceForDiscovery)
        {
            _fixtureFactory = fixtureFactory;
            _nameGenator = nameGenerator;
            _fixtureInstanceForDiscovery = fixtureInstanceForDiscovery;
        }
    }
}