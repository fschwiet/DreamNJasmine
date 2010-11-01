using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Misc
{
    [RequiresSTA]
    public class STA : NJasmineFixture
    {
        [STAThread]
        public override void Tests()
        {
            it("is STA", delegate
            {
                expect(System.Threading.Thread.CurrentThread.GetApartmentState()).to.Equal(ApartmentState.STA);
            });
        }
    }
}
