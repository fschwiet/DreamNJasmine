using System;
using NJasmineTests.Core;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    namespace WithNamespaceSetup
    {
        [Explicit("Stateful nature of this test prevents it from running more than once in the GUI runner")]
        public class supports_nunit_setup_by_namespace : GivenWhenThenFixtureTracingToConsole, INJasmineInternalRequirement
        {
            public override void Specify()
            {
                beforeAll(ResetTracing);

                it("test must run setup from NamespaceSetupB only, skipping NamespaceSetupA", delegate {
                    
                    Trace("running test 1");

                    expect(() => NamespaceSetupA.SetupCount == 0);
                    expect(() => NamespaceSetupB.SetupCount == 1);
                });

                it("should only run setup once", delegate
                {
                    Trace("running test 2");

                    expect(() => NamespaceSetupA.SetupCount == 0);
                    expect(() => NamespaceSetupB.SetupCount == 1);
                });
            }

            public void Verify(FixtureResult fixtureResult)
            {
                fixtureResult.succeeds();

                fixtureResult.containsTrace(@"
running test 1
running test 2
TearDown NamespaceSetupB
");
            }
        }

        [TestFixture]
        public class experimenting_with_nunit_namespace_setup
        {
            [Test]
            public void only_shows_second_namespace_setup_hmm()
            {
                
            }
        }

        [SetUpFixture]
        public class NamespaceSetupA
        {
            static public int SetupCount = 0;

            [SetUp]
            public void Setup()
            {
                SetupCount++;  // can't trace this step as it happens before trace reset
            }

            [TearDown]
            public void TearDown()
            {
                supports_nunit_setup_by_namespace.Trace("TearDown NamespaceSetupA");
            }
        }


        [SetUpFixture]
        public class NamespaceSetupB
        {
            static public int SetupCount = 0;

            [SetUp]
            public void Setup()
            {
                SetupCount++;  // can't trace this step as it happens before trace reset
            }

            [TearDown]
            public void TearDown()
            {
                supports_nunit_setup_by_namespace.Trace("TearDown NamespaceSetupB");
            }
        }
    }
}
