
namespace NJasmine.Core
{
    public interface INJasmineNameable
    {
        string Shortname { get; set; }
        string FullName { get; set; }
        string MultilineName { get; set; }
    }

    public interface INJasmineTest : INJasmineNameable
    {
        TestPosition Position { get; }
    }
}
