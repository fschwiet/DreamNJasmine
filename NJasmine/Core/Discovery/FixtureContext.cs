using System;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;
using NJasmine.Core.NativeWrappers;
using NJasmine.Extras;

namespace NJasmine.Core.Discovery
{
    public class FixtureContext
    {
        public GlobalSetupOwner GlobalSetupOwner { get; set; }
        public NameReservations NameReservations;
        public readonly INativeTestFactory NativeTestFactory;
        private SpecificationFixture _fixtureInstanceForDiscovery;
        public Func<SpecificationFixture> FixtureFactory;
        public IGlobalSetupManager GlobalSetupManager;

        public FixtureContext(INativeTestFactory nativeTestFactory, Func<SpecificationFixture> fixtureFactory, NameReservations nameReservations, GlobalSetupOwner globalSetupOwner, IGlobalSetupManager globalSetupManager)
        {
            GlobalSetupOwner = globalSetupOwner;
            NativeTestFactory = nativeTestFactory;
            FixtureFactory = fixtureFactory;
            NameReservations = nameReservations;
            _fixtureInstanceForDiscovery = fixtureFactory();
            GlobalSetupManager = globalSetupManager;
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

        public INativeTest CreateTest(INativeTest parentTest, TestPosition position, string description)
        {
            var testContext = new TestContext()
            {
                Name = NameReservations.GetReservedTestName(description, parentTest.Name),
                Position = position,
                FixtureContext = this
            };

            return NativeTestFactory.ForTest(this, testContext);
        }
    }
}