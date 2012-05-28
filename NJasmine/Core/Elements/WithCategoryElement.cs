using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class WithCategoryElement : SpecificationElement
    {
        readonly string _category;

        public WithCategoryElement(ActualKeyword actualKeyword, string category) : base(actualKeyword)
        {
            _category = category;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitWithCategory(this, _category, position);
            return ElementResultUnused;
        }
    }
}
