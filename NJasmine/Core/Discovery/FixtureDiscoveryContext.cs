﻿using System;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;
using NJasmine.Extras;

namespace NJasmine.Core.Discovery
{
    public class FixtureDiscoveryContext
    {
        public NameReservations NameGenator;
        private SpecificationFixture _fixtureInstanceForDiscovery;
        readonly INativeTestFactory _nativeTestFactory;
        private Func<SpecificationFixture> _fixtureFactory;

        public FixtureDiscoveryContext(INativeTestFactory nativeTestFactory, Func<SpecificationFixture> fixtureFactory, NameReservations nameReservations, SpecificationFixture fixtureInstanceForDiscovery)
        {
            _nativeTestFactory = nativeTestFactory;
            _fixtureFactory = fixtureFactory;
            NameGenator = nameReservations;
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

        public TestBuilder CreateTest(GlobalSetupManager globalSetupManager, TestBuilder parentTest, TestPosition position, string description)
        {
            var test = new TestBuilder(_nativeTestFactory.ForTest(_fixtureFactory, position, globalSetupManager));

            test.Name = NameGenator.GetReservedTestName(description, parentTest.Name);
            return test;
        }
    }
}