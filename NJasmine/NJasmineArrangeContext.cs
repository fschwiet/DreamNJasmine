using System;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    public class NJasmineArrangeContext : INJasmineArrangeContext
    {
        readonly ISpecVisitor _specVisitor;

        public NJasmineArrangeContext(ISpecVisitor specVisitor)
        {
            _specVisitor = specVisitor;
        }

        public ISpecVisitor SpecVisitor { get { return _specVisitor; } }

        public void beforeEach(Action action)
        {
            _specVisitor.visitBeforeEach(SpecElement.beforeEach, null, delegate()
            {
                action();
                return (string) null;
            });
        }

        public void afterEach(Action action)
        {
            _specVisitor.visitAfterEach(SpecElement.afterEach, action);
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return NUnitFixtureDriver.IncludeFixture<TFixture>(SpecVisitor);
        }

        public TArranged arrange<TArranged>() where TArranged : class, new()
        {
            return _specVisitor.visitBeforeEach(SpecElement.arrange, null, () => new TArranged());
        }

        public TArranged arrange<TArranged>(Func<TArranged> factory)
        {
            return _specVisitor.visitBeforeEach(SpecElement.arrange, null, factory);
        }

        public void arrange(Action action)
        {
            _specVisitor.visitBeforeEach(SpecElement.arrange, null, delegate
            {
                action();
                return (string) null;
            });
        }

        public void arrange(string description, Action action)
        {
            _specVisitor.visitBeforeEach(SpecElement.arrange, description, delegate()
            {
                action();
                return (string) null;
            });
        }
    }
}