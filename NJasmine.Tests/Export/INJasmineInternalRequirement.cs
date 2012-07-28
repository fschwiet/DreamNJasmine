using NJasmine.Marshalled;

namespace NJasmineTests.Export
{
    public interface INJasmineInternalRequirement
    {
        void Verify_NJasmine_implementation(IFixtureResult fixtureResult);
    }
}
