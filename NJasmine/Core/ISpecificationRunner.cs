using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core
{
    public interface ISpecificationRunner
    {
        TestPosition CurrentPosition { get; set; }
        ISpecPositionVisitor Visitor { get; set; }
        void Run();
    }
}