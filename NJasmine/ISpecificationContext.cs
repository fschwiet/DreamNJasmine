using System;

namespace NJasmine
{
    public interface ISpecificationContext
    {
        void ExtendSpec(Action<ISpecVisitor> spec);
    }
}