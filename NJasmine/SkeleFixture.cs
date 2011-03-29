using System;
using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    public class SkeleFixture : ISpecificationRunner
    {
        private readonly Action _specification;
        public TestPosition CurrentPosition { get; set; }
        public ISpecPositionVisitor Visitor { get; set; }

        public SkeleFixture(Action specification)
        {
            _specification = specification;
        }

        public void Run()
        {
            _specification();
        }
    }
}