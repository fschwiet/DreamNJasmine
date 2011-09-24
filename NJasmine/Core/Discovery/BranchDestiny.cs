using System;
using System.Collections.Generic;
using System.Linq;

namespace NJasmine.Core.Discovery
{
    public class BranchDestiny
    {
        public Queue<PendingDiscoveryBranches> _pendingDiscoveryBranches = new Queue<PendingDiscoveryBranches>();
        public TestPosition _destinedPath = new TestPosition();

        public void RunPendingDiscoveryBranches(Action action)
        {
            while (_pendingDiscoveryBranches.Any())
            {
                _destinedPath = _pendingDiscoveryBranches.Dequeue().ChosenPath;

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

        public int GetDiscoveriesQueuedCount()
        {
            return _pendingDiscoveryBranches.Count();
        }
    }
}