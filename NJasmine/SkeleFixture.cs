using System;
using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine
{
    public class SkeleFixture : ISpecificationRunner, ISpecificationContext
    {
        private readonly Action _specification;
        protected ISpecVisitor _visitor = new DoNothingFixtureVisitor();

        public SkeleFixture(Action specification)
        {
            _specification = specification;
        }

        public void Run()
        {
            _specification();
        }

        public virtual RunsActionOnDispose UseVisitor(ISpecVisitor visitor)
        {
            var currentVisitor = _visitor;

            _visitor = visitor;

            return new RunsActionOnDispose(() => _visitor = currentVisitor);
        }

        public void ExtendSpec(Action<ISpecVisitor> spec)
        {
            spec(_visitor);
        }
    }
}