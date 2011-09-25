using System;
using System.Collections.Generic;
using System.Linq;

namespace NJasmine.Core.Discovery
{
    public class BranchDestiny
    {
        Queue<PendingDiscoveryBranches> _pendingDiscoveryBranches = new Queue<PendingDiscoveryBranches>();
        TestPosition _destinedPath = new TestPosition();

        public void SetPredeterminedPath(TestPosition destinationPath)
        {
            _destinedPath = destinationPath;
        }

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

        public void AddRemainingOptions(TestPosition position, Action<Action>[] options)
        {
            InlineBranching.HandleInlineBranches(position, options, 
                (branch, branchPosition) => _pendingDiscoveryBranches.Enqueue(new PendingDiscoveryBranches()
                {
                    ChosenPath = branchPosition
                }), 
                skip:1);
        }
    }
}