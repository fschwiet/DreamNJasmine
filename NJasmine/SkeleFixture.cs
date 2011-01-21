using NJasmine.Core.FixtureVisitor;

namespace NJasmine
{
    public abstract class SkeleFixture : ExpectationsFixture
    {
        protected INJasmineFixtureVisitor _visitor = new DoNothingFixtureVisitor();

        public virtual NJasmineFixture.VisitorChangedContext UseVisitor(INJasmineFixtureVisitor visitor)
        {
            var currentVisitor = _visitor;

            _visitor = visitor;

            return new NJasmineFixture.VisitorChangedContext(() => _visitor = currentVisitor);
        }

        public abstract void Specify();
    }
}