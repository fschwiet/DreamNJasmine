using System;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core
{
    public abstract class SpecificationFixture 
    {
        public TestPosition CurrentPosition { get; set; }
        public ISpecPositionVisitor Visitor { get; set; }

        public void Run()
        {
            this.Specify();
        }

        public abstract void Specify();

        protected void RunSpecificationElement(SpecificationElement specElement)
        {
            RunSpecificationElement<object>(specElement);
        }

        protected T RunSpecificationElement<T>(SpecificationElement specificationElement)
        {
            T result = default(T);

            var position1 = CurrentPosition;
            var nextPosition = CurrentPosition.GetNextSiblingPosition();
            CurrentPosition = CurrentPosition.GetFirstChildPosition();
            result = ((Func<TestPosition, T>) (position => { return (T)specificationElement.Run(Visitor, position); }))(position1);
            CurrentPosition = nextPosition;

            return result;
        }
    }
}