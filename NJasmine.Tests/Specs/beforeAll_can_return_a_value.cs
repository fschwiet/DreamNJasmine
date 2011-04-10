using NJasmine;

namespace NJasmineTests.Specs
{
    public class beforeAll_can_return_a_value : GivenWhenThenFixture
    {
        public class SomeClass
        {
            public int Value;
        }

        public override void Specify()
        {
            var perRunFixture = beforeAll(delegate
            {
                return new SomeClass() {Value = 0};
            });

            var perTestFixture = new SomeClass() {Value = 0};

            then("the value is initialized for the first test", delegate
            {
                expect(() => perRunFixture.Value == 0);

                perRunFixture.Value++;

                expect(() => perTestFixture.Value == 0);
                perTestFixture.Value++;
            });

            then("the value is reused for the second test", delegate
            {
                expect(() => perRunFixture.Value == 1);

                perRunFixture.Value++;

                expect(() => perTestFixture.Value == 0);
                perTestFixture.Value++;
            });

            then("the value is reused for the third test", delegate
            {
                expect(() => perRunFixture.Value == 2);

                expect(() => perTestFixture.Value == 0);
            });
        }
    }
}
