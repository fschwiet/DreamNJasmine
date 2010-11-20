NOTE:  NJasmine is known to work with 2.5.7.  There is a known issue preventing it from working with 2.5.8+, which I need to fix.  If that matters to you, please open something in the issue tracker.


NJasmine is a port of a subset of JasmineBDD language.  At the moment, it supports describe(), beforeEach(), afterEach(), it(), and expect().

NJasmine uses Should.Fluent.dll for its matching.  It exposes Should.Fluent via expect, but this has limitations.  Prefer expressing expectations as N.Should() instead of expect(N) until I can fix this issue.

NJasmine also supports cleanup of test resources that use IDisposable.  Call disposing<TDisposable>() or disposing(Action<TDisposable>) to load such resources and they will automatically be Dispose()d.

NJasmine also supports including traditional NUnit test fixtures setup.  If you have fixtures that do resource cleanup via [SetUp], [FitureSetUp], [TearDown] or [FixtureTeardown], you can call importNUnit<TFixture>() to load such fixture within the test.  Note that the fixture will need to make members public to be accessible.

NJasmine does not support JasmineBDD's spies or asynchronous testing.  I may add asynchronous support in the future, but I don't think spies will be viable.  Instead, use the .Net mocking framework of your choice.

See "how to build.txt" for information on how to build and install NJasmine.