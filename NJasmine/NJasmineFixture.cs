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
    public abstract class NJasmineFixture : ExpectationsFixture
    {
        INJasmineFixtureVisitor _visitor = new DoNothingFixtureVisitor();
        readonly Stack<INJasmineFixtureVisitor> _visitorStack = new Stack<INJasmineFixtureVisitor>();
 
        public void PushVisitor(INJasmineFixtureVisitor visitor)
        {
            _visitorStack.Push(_visitor);

            _visitor = visitor;
        }

        public void PopVisitor()
        {
            _visitor = _visitorStack.Pop();
        }

        public void ClearVisitor()
        {
            _visitor = new DoNothingFixtureVisitor();
        }

        public abstract void Tests();

        protected void describe(string description)
        {
            describe(description, null);
        }

        protected void describe(string description, Action action)
        {
            _visitor.visitDescribe(description, action);
        }

        protected void beforeEach(Action action)
        {
            _visitor.visitBeforeEach(action);
        }

        protected void afterEach(Action action)
        {
            _visitor.visitAfterEach(action);
        }

        protected void it(string description)
        {
            _visitor.visitIt(description, null);
        }

        protected void it(string description, Action action)
        {
            _visitor.visitIt(description, action);
        }

        protected TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return _visitor.visitImportNUnit<TFixture>();
        }

        protected TArranged arrange<TArranged>() where TArranged : class, new()
        {
            Func<TArranged> factory = delegate
            {
                return new TArranged();
            };

            return _visitor.visitArrange<TArranged>(factory);
        }

        protected TArranged arrange<TArranged>(Func<TArranged> factory)
        {
            return _visitor.visitArrange<TArranged>(factory);
        }

        protected void ignore(Action action)
        {
        }

        protected void ignore(string message, Action action)
        {
        }
    }
}
