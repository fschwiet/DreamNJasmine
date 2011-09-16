using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs.cleanup_any_IDisposable
{
    [Explicit]
    public class leakDisposable_can_be_used_to_prevent_automatic_dispose : GivenWhenThenFixture
    {
        static public List<string> DisposesCalled;

        class DisposeTracker : IDisposable
        {
            readonly string _name;

            public DisposeTracker(string name)
            {
                _name = name;
            }

            public void Dispose()
            {
                DisposesCalled.Add(_name);
            }
        }

        public override void Specify()
        {
            beforeAll(() => DisposesCalled = new List<string>());

            describe("beforeAll results can be leaked", delegate
            {
                var leakedOnce = beforeAll(() => new DisposeTracker("leakedOnce"));
                var leakedOnce2 = beforeAll(() => new DisposeTracker("leakedOnce"));
                var disposedOnce = beforeAll(() => new DisposeTracker("disposedOnce"));

                leakDisposable(leakedOnce);

                it("some test", delegate()
                {
                    leakDisposable(leakedOnce2);

                    expect(() => leakedOnce != null);
                    expect(() => disposedOnce != null);
                });
            });

            describe("before results can be leaked", delegate
            {
                var leakedEach = beforeEach(() => new DisposeTracker("leakedEach"));
                var leakedEach2 = beforeEach(() => new DisposeTracker("leakedEach"));
                var disposedEach = beforeEach(() => new DisposeTracker("disposedEach"));

                leakDisposable(leakedEach);

                it("some test", delegate()
                {
                    leakDisposable(leakedEach2);

                    expect(() => leakedEach != null);
                    expect(() => disposedEach != null);
                });            
            });

            it("leaked the disposables in this test", delegate()
            {
                Assert.That(DisposesCalled, Is.EquivalentTo(new string[] {"disposedOnce", "disposedEach"}));
            });
        }

        void leakDisposable(IDisposable disposable)
        {
            throw new NotImplementedException();
        }
    }
}
