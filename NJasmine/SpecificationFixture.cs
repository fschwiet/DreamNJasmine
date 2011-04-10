using System;
using System.Linq.Expressions;
using System.Threading;
using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine
{
    public abstract class SpecificationFixture 
    {
        internal TestPosition CurrentPosition { get; set; }
        internal ISpecPositionVisitor Visitor { get; set; }

        public SpecificationFixture()
        {
        }

        public void Run()
        {
            this.Specify();
        }

        public abstract void Specify();
    }
}