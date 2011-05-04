using NJasmine;

namespace NJasmineTests.Specs
{
    public class constructor_is_run_once_per_test : GivenWhenThenFixture
    {
        public int? LastValue;
        public constructor_is_run_once_per_test LastInstance;
        public int Value = 0;

        public override void Specify()
        {
            for (var i = 0; i < 10; i++)
            {
                it("no state is saved from a previous run", () => expect(() => !LastValue.HasValue));

                it("new instance is created per test", () => expect(() => LastInstance == null));

                it("writes state in some tests", delegate
                {
                    LastValue = 1;
                    LastInstance = this;
                });
            }
        }
    }
}
