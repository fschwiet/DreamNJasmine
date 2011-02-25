using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;
using NUnit.Core;
using NUnit.Framework;
using Should.Fluent;
using Should.Fluent.Model;
using Assert = Should.Core.Assertions.Assert;

namespace NJasmine
{
    public abstract class NJasmineFixture : SpecificationFixture
    {
        public NJasmineFixture()
        {}

        protected NJasmineFixture(SkeleFixture fixture) : base(fixture)
        {}

        public void describe(string description)
        {
            describe(description, null);
        }

        public void describe(string description, Action action)
        {
            _skeleFixture.ExtendSpec(s => s.visitFork(SpecElement.describe, description, action));
        }

        public void beforeAll(Action action)
        {
            _skeleFixture.ExtendSpec(s => s.visitBeforeAll(SpecElement.beforeAll, delegate()
            {
                action();
                return (string)null;
            }));
        }

        public void beforeEach(Action action)
        {
            _skeleFixture.ExtendSpec(s => s.visitBeforeEach(SpecElement.beforeEach, null, delegate() { action(); return (string)null; }));
        }

        public void afterEach(Action action)
        {
            _skeleFixture.ExtendSpec(s => s.visitAfterEach(SpecElement.afterEach, action));
        }

        public void it(string description)
        {
            _skeleFixture.ExtendSpec(s => s.visitTest(SpecElement.it, description, null));
        }

        public void it(string description, Action action)
        {
            _skeleFixture.ExtendSpec(s => s.visitTest(SpecElement.it, description, action));
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return NUnitFixtureDriver.IncludeFixture<TFixture>(_skeleFixture);
        }

        public TArranged arrange<TArranged>() where TArranged : class, new()
        {
            Func<TArranged> factory = delegate
            {
                return new TArranged();
            };

            TArranged result = default(TArranged);

            _skeleFixture.ExtendSpec(s => result = s.visitBeforeEach(SpecElement.arrange, null, factory));

            return result;
        }

        public TArranged arrange<TArranged>(Func<TArranged> factory)
        {
            TArranged result = default(TArranged);
            _skeleFixture.ExtendSpec(s => result = s.visitBeforeEach(SpecElement.arrange, null, factory));
            return result;
        }

        public void arrange(Action action)
        {
            arrange(null, action);
        }

        public void arrange(string description, Action action)
        {
            _skeleFixture.ExtendSpec(s => s.visitBeforeEach(SpecElement.arrange, description, delegate() { action(); return (string)null; }));
        }
    }
}
