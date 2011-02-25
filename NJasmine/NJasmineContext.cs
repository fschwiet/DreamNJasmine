using System;

namespace NJasmine
{
    public class NJasmineContext : NJasmineFixture
    {
        public NJasmineContext(SkeleFixture fixture) : base(fixture)
        {
        }

        public override void Specify()
        {
            throw new NotImplementedException();
        }
    }
}