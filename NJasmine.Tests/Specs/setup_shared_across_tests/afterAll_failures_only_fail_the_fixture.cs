using NJasmine;

namespace NJasmineTests.Specs.setup_shared_across_tests
{

    //
    // I'm not totally happy with this behavior, but it seems like the best
    // I can do as NUnit ignores fixture failures
    //

    // Note: it looks like NUnit.2.5.10 will report these...

    public class afterAll_failures_only_fail_the_fixture : GivenWhenThenFixture
    {
        public override void Specify()
        {
            when("cleanup is going to fail", delegate
            {
                afterAll(delegate
                {
                    expect(() => true == false);
                });

                then("the test with failing cleanup", delegate
                {
                                                              
                });
            });

            then("the test following the test with failing cleanup", delegate
            {
                
            });

            afterAll(delegate
            {
                expect(() => 123 > 456);                
            });

            then("a test with failing cleanup and no following tests", delegate
            {
                                                                               
            });
        }
    }
}
