NOTE:  NJasmine current only works with NUnit 2.5.9.

NJasmine is a port of a subset of JasmineBDD language to .NET in C#.  At the moment, it supports describe(), beforeEach(), afterEach(), and it().

NJasmine also supports cleanup of test resources that use IDisposable.  Call arrange<TDisposable>() or arrange(Action<TDisposable>) to load such resources and they will automatically be Dispose()d.  You may call arrange() outside of beforeEach(), it().

NJasmine also supports including traditional NUnit test fixtures.  If you have fixtures that do resource cleanup via [SetUp], [TestFixtureSetUp], [TearDown] or [TestFixtureTeardown], you can call importNUnit<TFixture>() to load such fixture within the test.  Note that the fixture will need to make members public to be accessible.

NJasmine does not support JasmineBDD's spies or asynchronous testing.  I may add asynchronous support in the future, for now just block in the test thread.  I don't think spies will be viable, instead use the .Net mocking framework of your choice.

See "how to build.txt" for information on how to build and install NJasmine.