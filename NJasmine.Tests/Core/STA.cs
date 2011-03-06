using System;
using System.Threading;
using NJasmine;
using NUnit.Framework;
using Should.Fluent;

namespace NJasmineTests.Core
{
    [RequiresSTA]
    public class STA : NJasmineFixture
    {
        public override void Specify()
        {
            it("is STA", delegate
            {
                System.Threading.Thread.CurrentThread.GetApartmentState().Should().Equal(ApartmentState.STA);
            });
        }
    }
}
