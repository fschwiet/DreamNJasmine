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
    public abstract class NJasmineFixture : SkeleFixture
    {
        public void describe(string description)
        {
            describe(description, null);
        }

        public void describe(string description, Action action)
        {
            SpecVisitor.visitFork(SpecElement.describe, description, action);
        }

        public void beforeAll(Action action)
        {
            SpecVisitor.visitBeforeAll(SpecElement.beforeAll, delegate()
            {
                action();
                return (string)null;
            });
        }

        public void beforeEach(Action action)
        {
            SpecVisitor.visitBeforeEach(SpecElement.beforeEach, null, delegate() { action(); return (string)null; });
        }

        public void afterEach(Action action)
        {
            SpecVisitor.visitAfterEach(SpecElement.afterEach, action);
        }

        public void it(string description)
        {
            SpecVisitor.visitTest(SpecElement.it, description, null);
        }

        public void it(string description, Action action)
        {
            SpecVisitor.visitTest(SpecElement.it, description, action);
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return NUnitFixtureDriver.IncludeFixture<TFixture>(SpecVisitor);
        }

        public TArranged arrange<TArranged>() where TArranged : class, new()
        {
            Func<TArranged> factory = delegate
            {
                return new TArranged();
            };

            return SpecVisitor.visitBeforeEach<TArranged>(SpecElement.arrange, null, factory);
        }

        public TArranged arrange<TArranged>(Func<TArranged> factory)
        {
            return SpecVisitor.visitBeforeEach<TArranged>(SpecElement.arrange, null, factory);
        }

        public void arrange(Action action)
        {
            arrange(null, action);
        }

        public void arrange(string description, Action action)
        {
            SpecVisitor.visitBeforeEach<string>(SpecElement.arrange, description, delegate() { action(); return (string)null; });
        }
    }
}
