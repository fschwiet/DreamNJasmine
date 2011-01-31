using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;
using NUnit.Framework;
using Should.Fluent;
using Should.Fluent.Model;
using Assert = Should.Core.Assertions.Assert;

namespace NJasmine
{
    public abstract class NJasmineFixture : SkeleFixture, IArrangeContext
    {
        public void describe(string description)
        {
            describe(description, null);
        }

        public void describe(string description, Action action)
        {
            _visitor.visitFork(SpecElement.describe, description, action);
        }

        public void beforeEach(Action action)
        {
            _visitor.visitBeforeEach(SpecElement.beforeEach, null, delegate() { action(); return (string)null; });
        }

        public void afterEach(Action action)
        {
            _visitor.visitAfterEach(action);
        }

        public void it(string description)
        {
            _visitor.visitTest(description, null);
        }

        public void it(string description, Action action)
        {
            _visitor.visitTest(description, action);
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return _visitor.visitImportNUnit<TFixture>();
        }

        public TArranged arrange<TArranged>() where TArranged : class, new()
        {
            Func<TArranged> factory = delegate
            {
                return new TArranged();
            };

            return _visitor.visitBeforeEach<TArranged>(SpecElement.arrange, null, factory);
        }

        public TArranged arrange<TArranged>(Func<TArranged> factory)
        {
            return _visitor.visitBeforeEach<TArranged>(SpecElement.arrange, null, factory);
        }

        public void arrange(Action action)
        {
            arrange(null, action);
        }

        public void arrange(string description, Action action)
        {
            _visitor.visitBeforeEach<string>(SpecElement.arrange, description, delegate() { action(); return (string)null; });
        }
    }
}
