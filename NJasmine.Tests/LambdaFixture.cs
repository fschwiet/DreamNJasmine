using System;
using NJasmine;

namespace NJasmineTests
{
    class LambdaFixture : GivenWhenThenFixture
    {
        public Action<LambdaFixture> LambdaSpecify;

        public override void Specify()
        {
            LambdaSpecify(this);
        }
    }
}