using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace NJasmineTests.Specs.proposed_specs.sharing_context
{
    abstract class SharingFixtureBase : GivenWhenThenFixture
    {
        public T with<T>(string withBrowser) where T : SharedFixtureBase
        {
            return null;
        }
    }
}
