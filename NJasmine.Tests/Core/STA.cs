using System;
using System.Threading;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    [RequiresSTA]
    public class STA : GivenWhenThenFixture
    {
        public override void Specify()
        {
            it("is STA", delegate
            {
                expect(() => System.Threading.Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);
            });
        }
    }
}
