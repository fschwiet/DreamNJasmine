using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests
{
    class TestOnlyUsingDescribeAndIt : NJasmineFixture
    {
        public override void Tests()
        {


it("first test", delegate() { });               

describe("first describe", delegate()
{
    it("first inner test", delegate() { });

    it("second inner test", delegate() { });

    describe("second describe", delegate()
    {
        it("first inner-inner test", delegate() { });

        it("second inner-inner test", delegate() { });
    });
});


        }
    }
}
