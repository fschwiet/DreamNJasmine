using NJasmine.Core;
using NJasmine.Extras;

namespace NJasmine
{
    public interface ISpecificationRunner
    {
        RunsActionOnDispose UseVisitor(ISpecVisitor visitor);
        void Run();
    }
}