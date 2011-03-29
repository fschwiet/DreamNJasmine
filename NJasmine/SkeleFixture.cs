using System;
using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    public class SkeleFixture : ISpecificationRunner, ISpecificationContext
    {
        private readonly Action _specification;
        public ISpecVisitor Visitor { get; protected set; }

        public SkeleFixture(Action specification)
        {
            Visitor = new DoNothingFixtureVisitor();
            _specification = specification;
        }

        public void Run()
        {
            _specification();
        }

        public RunsActionOnDispose UseVisitor(ISpecVisitor visitor)
        {
            var currentVisitor = Visitor;

            Visitor = visitor;

            return new RunsActionOnDispose(() => Visitor = currentVisitor);
        }
    }
}