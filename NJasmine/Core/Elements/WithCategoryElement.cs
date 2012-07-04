using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class WithCategoryElement : SpecificationElement
    {
        public readonly string Category;

        public WithCategoryElement(ActualKeyword actualKeyword, string category) : base(actualKeyword)
        {
            Category = category;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitWithCategory(this, position);
            return ElementResultUnused;
        }
    }
}
