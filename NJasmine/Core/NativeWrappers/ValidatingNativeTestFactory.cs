using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;

namespace NJasmine.Core.NativeWrappers
{
    public class ValidatingNativeTestFactory : INativeTestFactory
    {
        private readonly INativeTestFactory _factory;

        public ValidatingNativeTestFactory(INativeTestFactory factory)
        {
            _factory = factory;
        }

        public void SetRoot(INativeTest test)
        {
            _factory.SetRoot(test);
        }

        public INativeTest ForSuite(FixtureContext fixtureContext, TestContext testContext)
        {
            return new ValidatingNativeTestWrapper(_factory.ForSuite(fixtureContext, testContext));
        }

        public INativeTest ForTest(FixtureContext fixtureContext, TestContext testContext)
        {
            return new ValidatingNativeTestWrapper(_factory.ForTest(fixtureContext, testContext));
        }
    }
}
