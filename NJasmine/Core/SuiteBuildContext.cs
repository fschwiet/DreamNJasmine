using System;

namespace NJasmine.Core
{
    internal class SuiteBuildContext
    {
        public Func<ISpecificationRunner> _fixtureFactory;
        public ISpecificationRunner _fixtureInstanceForDiscovery;
        public NameGenerator _nameGenator;

        public SuiteBuildContext(Func<ISpecificationRunner> fixtureFactory, NameGenerator nameGenerator, ISpecificationRunner fixtureInstanceForDiscovery)
        {
            _fixtureFactory = fixtureFactory;
            _nameGenator = nameGenerator;
            _fixtureInstanceForDiscovery = fixtureInstanceForDiscovery;
        }
    }
}