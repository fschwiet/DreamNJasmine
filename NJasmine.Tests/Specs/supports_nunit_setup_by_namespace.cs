using NJasmineTests.Core;
using NUnit.Framework;
using Should.Fluent;

namespace NJasmineTests.Specs
{
    namespace WithNamespaceSetup
    {
        [RunExternal(true, ExpectedTraceSequence = @"
running test 1
running test 2
TearDown NamespaceSetupB
")]
        [Explicit("Stateful nature of this test prevents it from running more than once in the GUI runner")]
        public class supports_nunit_setup_by_namespace : GivenWhenThenFixtureTracingToConsole
        {
            public override void Specify()
            {
                beforeAll(ResetTracing);

                it("test must run setup from NamespaceSetupB only, skipping NamespaceSetupA", delegate {
                    
                    Trace("running test 1");

                    NamespaceSetupA.SetupCount.Should().Equal(0);
                    NamespaceSetupB.SetupCount.Should().Equal(1);
                });

                it("should only run setup once", delegate
                {
                    Trace("running test 2");

                    NamespaceSetupA.SetupCount.Should().Equal(0);
                    NamespaceSetupB.SetupCount.Should().Equal(1);
                });
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
