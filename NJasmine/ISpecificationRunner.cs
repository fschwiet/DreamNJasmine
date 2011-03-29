using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    public interface ISpecificationRunner
    {
        TestPosition CurrentPosition { get; set; }
        ISpecPositionVisitor Visitor { get; set; }
        void Run();
    }
}