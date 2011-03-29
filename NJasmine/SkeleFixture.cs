using System;
using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    public class SkeleFixture : ISpecificationRunner, ISpecificationContext
    {
        private readonly Action _specification;
        public TestPosition CurrentPosition { get; set; }
        public ISpecPositionVisitor Visitor { get; set; }

        public SkeleFixture(Action specification)
        {
            Visitor = new DoNothingFixtureVisitor();
            _specification = specification;
        }

        public void Run()
        {
            _specification();
        }
    }
}