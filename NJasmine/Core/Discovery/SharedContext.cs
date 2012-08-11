using System;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;
using NJasmine.Extras;

namespace NJasmine.Core.Discovery
{
    public class SharedContext
    {
        public NameReservations NameReservations;
        public readonly INativeTestFactory NativeTestFactory;
        private SpecificationFixture _fixtureInstanceForDiscovery;
        public Func<SpecificationFixture> FixtureFactory;

        public SharedContext(INativeTestFactory nativeTestFactory, Func<SpecificationFixture> fixtureFactory, NameReservations nameReservations, SpecificationFixture fixtureInstanceForDiscovery)
        {
            NativeTestFactory = nativeTestFactory;
            FixtureFactory = fixtureFactory;
            NameReservations = nameReservations;
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

        public TestBuilder CreateTest(IGlobalSetupManager globalSetupManager, TestBuilder parentTest, TestPosition position, string description)
        {
            var testContext = new TestContext()
            {
                Name = NameReservations.GetReservedTestName(description, parentTest.Name),
                Position = position,
                GlobalSetupManager = globalSetupManager
            };

            var test = new TestBuilder(NativeTestFactory.ForTest(this, testContext), testContext.Name);

            return test;
        }
    }
}