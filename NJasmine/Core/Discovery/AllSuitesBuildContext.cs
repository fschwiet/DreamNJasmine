using System;
using System.Collections.Generic;

namespace NJasmine.Core.Discovery
{
    internal class AllSuitesBuildContext
    {
        public Func<SpecificationFixture> _fixtureFactory;
        public SpecificationFixture _fixtureInstanceForDiscovery;
        public NameGenerator _nameGenator;
        public List<PendingDiscoveryBranches> _pendingDiscoveryBranches;

        public AllSuitesBuildContext(Func<SpecificationFixture> fixtureFactory, NameGenerator nameGenerator, SpecificationFixture fixtureInstanceForDiscovery)
        {
            _fixtureFactory = fixtureFactory;
            _nameGenator = nameGenerator;
            _fixtureInstanceForDiscovery = fixtureInstanceForDiscovery;
            _pendingDiscoveryBranches = new List<PendingDiscoveryBranches>();
        }

        public void RunPendingDiscoveryBranches(Action action)
        {
            foreach (var pending in _pendingDiscoveryBranches)
            {

            }
        }
    }

    class PendingDiscoveryBranches
    {
    }
}