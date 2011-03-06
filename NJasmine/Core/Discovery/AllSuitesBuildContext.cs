using System;

namespace NJasmine.Core.Discovery
{
    internal class AllSuitesBuildContext
    {
        public Func<ISpecificationRunner> _fixtureFactory;
        public ISpecificationRunner _fixtureInstanceForDiscovery;
        public NameGenerator _nameGenator;

        public AllSuitesBuildContext(Func<ISpecificationRunner> fixtureFactory, NameGenerator nameGenerator, ISpecificationRunner fixtureInstanceForDiscovery)
        {
            _fixtureFactory = fixtureFactory;
            _nameGenator = nameGenerator;
            _fixtureInstanceForDiscovery = fixtureInstanceForDiscovery;
        }
    }
}