using System;
using System.Threading;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    [RequiresSTA]
    public class STA : NJasmineFixture
    {
        [STAThread]
        public override void Specify()
        {
            it("is STA", delegate
            {
                expect(System.Threading.Thread.CurrentThread.GetApartmentState()).to.Equal(ApartmentState.STA);
            });
        }
    }
}
