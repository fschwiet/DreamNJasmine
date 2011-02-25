using NJasmine.Core;

namespace NJasmine
{
    public interface ISpecificationRunner
    {
        RunsActionOnDispose UseVisitor(ISpecVisitor visitor);
        void Run();
    }
}