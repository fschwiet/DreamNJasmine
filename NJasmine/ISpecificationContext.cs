using System;

namespace NJasmine
{
    public interface ISpecificationContext
    {
        ISpecVisitor Visitor { get; }
    }
}