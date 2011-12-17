using System;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine.Core.Discovery
{
    internal class FixtureDiscoveryContext
    {
        private SpecificationFixture _fixtureInstanceForDiscovery;

        public Func<SpecificationFixture> FixtureFactory;
        public NameGenerator NameGenator;

        public FixtureDiscoveryContext(Func<SpecificationFixture> fixtureFactory, NameGenerator nameGenerator, SpecificationFixture fixtureInstanceForDiscovery)
        {
            FixtureFactory = fixtureFactory;
            NameGenator = nameGenerator;
            _fixtureInstanceForDiscovery = fixtureInstanceForDiscovery;
        }

        public Action GetSpecificationRootAction()
        {
            return _fixtureInstanceForDiscovery.Run;
        }

        public Exception RunActionWithVisitor(TestPosition position, Action action, ISpecPositionVisitor visitor)
        {
            Exception exception = null;

            TestPosition firstChildPosition = position;

            var originalVisitor = _fixtureInstanceForDiscovery.Visitor;

            _fixtureInstanceForDiscovery.CurrentPosition = firstChildPosition;
            _fixtureInstanceForDiscovery.Visitor = visitor;

            try
            {
                action();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                _fixtureInstanceForDiscovery.Visitor = originalVisitor; 
            }

            return exception;
        }
    }
}