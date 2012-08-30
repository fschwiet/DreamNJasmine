using System;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;
using NJasmine.Core.NativeWrappers;
using NJasmine.Extras;

namespace NJasmine.Core.Discovery
{
    public class FixtureContext
    {
        public NameReservations NameReservations;
        public readonly INativeTestFactory NativeTestFactory;
        private SpecificationFixture _fixtureInstanceForDiscovery;
        public Func<SpecificationFixture> FixtureFactory;

        public FixtureContext(INativeTestFactory nativeTestFactory, Func<SpecificationFixture> fixtureFactory, NameReservations nameReservations)
        {
            NativeTestFactory = nativeTestFactory;
            FixtureFactory = fixtureFactory;
            NameReservations = nameReservations;
            _fixtureInstanceForDiscovery = fixtureFactory();
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

        public INativeTest CreateTest(IGlobalSetupManager globalSetupManager, INativeTest parentTest, TestPosition position, string description)
        {
            var testContext = new TestContext()
            {
                Name = NameReservations.GetReservedTestName(description, parentTest.Name),
                Position = position,
                GlobalSetupManager = globalSetupManager
            };

            return NativeTestFactory.ForTest(this, testContext);
        }
    }
}