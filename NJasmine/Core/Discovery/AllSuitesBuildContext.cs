using System;
using System.Collections.Generic;
using System.Linq;

namespace NJasmine.Core.Discovery
{
    internal class AllSuitesBuildContext
    {
        public Func<SpecificationFixture> _fixtureFactory;
        public SpecificationFixture _fixtureInstanceForDiscovery;
        public NameGenerator _nameGenator;
        public List<PendingDiscoveryBranches> _pendingDiscoveryBranches;
        public TestPosition _destinedPath = new TestPosition();

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
                _destinedPath = pending.ChosenPath;

                action();
            }
        }

        public int? GetDestinedPath(TestPosition position)
        {
            if (position.IsAncestorOf(_destinedPath))
            {
                return _destinedPath.Coordinates.ToArray()[position.Coordinates.Count()];
            }

            return null;
        }
    }

    class PendingDiscoveryBranches
    {
        public TestPosition ChosenPath;
    }
}