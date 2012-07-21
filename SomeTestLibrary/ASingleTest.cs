using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace SomeTestLibrary
{
    public class ASingleTest : GivenWhenThenFixture
    {
        public override void Specify()
        {
            it("first test");
        }
    }
}
