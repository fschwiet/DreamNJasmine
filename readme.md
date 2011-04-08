
NJasmine is a RSpec-ish test language inspired by the javasscript test library Jasmine (http://pivotal.github.com/jasmine/).

    given("some preconditions", () => {

        var range = 10;

        when("the system under test is ran", () => {

            var sut = new SystemUnderTest();

            bool score = arrange(() => sut.Fire(range));

            then("win!", () => {

                expect(() => score);
            });
        });
    });

Like Jasmine, NJasmine supports the describe() and it() test constructs.

Arrange can be carried out with beforeEach(), afterEach(), beforeAll() and afterAll().  
arrange() and cleanup() behave the same as beforeEach() and afterEach().  
The setup arrange methods can return a value to be used during the test.  
If a setup method returns a value that supports IDisposeable, the value is dispose()d when it leaves scope.

For given/when/then style tests, given() and when() behave like describe().  Likewise, then() behaves like it().

Assertions can be expressed as expect() or waitUntil().

The outer scope of each specification is ran once to discover what tests are defined, and then again once again for each test.  Test code must not block if its within describe(), given, or when() expression.  Such code will cause test discovery to stall.  Exceptions thrown within test discovery will show as failures to the test runner, but the test runner will not discover all tests.

If you would like to reuse test setup code made for NUnit, consider importNUnit<T>() (T is the NUnit fixture).  importNUnit<>() supports classes that use [SetUp], [TestFixtureSetUp], [TearDown] or [TestFixtureTeardown].

See "how to build.txt" for information on how to build and install NJasmine.

Currently only built against NUnit 2.5.9.  If you would use something else let me know.