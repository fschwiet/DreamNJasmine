using System;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine
{
    public interface ISpecificationContext
    {
        ISpecPositionVisitor Visitor { get; set; }
    }
}