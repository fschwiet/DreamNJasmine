### Introducing NJasmine

Available on Nuget: http://nuget.org/List/Packages/NJasmine

NJasmine is a RSpec-ish test language inspired by the javascript test library Jasmine (https://jasmine.github.io/) for C# / .Net programming.

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

### Why another .Net BDD testing framework?  

I classify the existing frameworks into two categories: frameworks like Rail's Cucumber test framework, and frameworks like Rail's RSpec test framework.

The cucumber test frameworks for .NET (SpecFlow, NBehave) are a different beast entirely.  I honestly just haven't gone that far into BDD.  The cucumber style tests do a good job of producing specs that are human readable, but there is a good amount of overhead before you actually write code.  I just haven't been interested in going there, I see such tools as complementary to the RSpec style frameworks.

Of the .NET RSpec style test frameworks, none that I've seen are what I'd really consider a DSL, as RSpec was.  They mostly rely on inheritance to accumulate specification context.  I don't like this as it breaks simple tests into multiple classes, and I find friction reusing test code because C# classes can only inherit from a single class.

NJasmine, like RSpec, is actually a DSL.  It reads easier [at least, it reads well for me :)], and the code feels more malleable to refactoring techniques.


### A birds-eye view of the NJasmine test language

Like Jasmine, NJasmine supports the describe() and it() test constructs.  Most test constructs in NJasmine have synonyms which are functionally equivalent but allow the test author to express different semantics.  For instance, one can using when() or given() in place of describe() to distinguish whether something is part of test setup or the test itself.  Some synonyms do affect the test name as well, for instance describe("when foo",...) will create a test with the same name as when("foo",...).

Arrange operations can be carried out with beforeEach(), afterEach(), beforeAll() and afterAll().  
arrange() and act() are synonyms for beforeEach().
beforeAll(), beforeEach() and synonyms can return a value which will be passed on to the test.  If the value returned supports IDisposeable, the value is dispose()d when it leaves scope.
cleanup() is a synonym for afterEach().

For given/when/then style tests, given() and when() are synonyms for describe().  Likewise, then() is a synonym for it().

Assertions can be expressed as expect(), expectEventually() or waitUntil().  expect() checks the assertion immediately.  expectEventually() and waitUntil() will recheck the assertion until it passes or times out.

The outer scope of each specification is ran once to discover what tests are defined, and then again once again for each contained test.  Test code must not block if its directly within a describe(), given, or when() expression.  Such code will cause test discovery to stall.

Unlike Jasmine or RSpec, in NJasmine each level of scope for a test runs for every test.  If you really want some setup code to run once for multiple tests, use beforeAll().  The expression passed to beforeAll() is invoked once then any value it returns is passed to the following tests.

If you would like to reuse test setup code made for NUnit, consider importNUnit<>().  importNUnit<>() supports classes that use [SetUp], [TestFixtureSetUp], [TearDown] or [TestFixtureTeardown].  NJasmine will let you include multiple setup fixtures in the same specification.

Tests can be categorized using withCategory(<name>).  The NUnit test runners allow you to exclude or include tests by category.

You can use existing NUnit test attributes [RequiresSTA] and [Explict] with the GivenWhenThenFixture.

### Getting started

The preferred means to installing NJasmine is with Nuget, available at http://nuget.org/List/Packages/NJasmine.  Or, use the download link has NJasmine binaries for download, where instructions were included in the zip.

See "how to build.md" for information on how to build NJasmine from source.

Currently only built against NUnit 2.5.9, 2.5.10 and 2.6.0.  If you would use something else let me know.  I use 2.5.10.  If you use a different version of the NUnit test runner then what NJasmine was built against, NUnit won't see your tests.
