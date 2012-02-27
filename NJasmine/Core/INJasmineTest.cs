
namespace NJasmine.Core
{
    public interface INJasmineNameable
    {
    }

    public interface INJasmineTest : INJasmineNameable
    {
        TestPosition Position { get; }
    }
}
