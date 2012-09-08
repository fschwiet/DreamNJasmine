using NJasmine;

namespace NJasmineTests.Specs.proposed_specs.sharing_context
{
    public abstract class GivenWhenThenFixture2 : GivenWhenThenFixture
    {
        public TContext includeContext<TContext>()
        {
            return default(TContext);
        }
    }

    public abstract class GivenWhenThenContext : GivenWhenThenFixture2
    {
        protected void go()
        {
        }

        // careful to not allow other specification code to call into given/when/then etc from other thread
    }

}
