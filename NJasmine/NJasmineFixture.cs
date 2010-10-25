using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core;
using NJasmine.FixtureVisitor;
using NUnit.Framework;
using Should.Fluent;
using Should.Fluent.Model;
using Assert = Should.Core.Assertions.Assert;

namespace NJasmine
{
    public abstract class NJasmineFixture : ExpectationsFixture
    {
        INJasmineFixtureVisitor _visitor = new DoNothingFixtureVisitor();

        public void SetVisitor(INJasmineFixtureVisitor visitor)
        {
            _visitor = visitor;
        }

        public void ClearVisitor()
        {
            _visitor = new DoNothingFixtureVisitor();
        }

        public abstract void Tests();

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

        protected void forEach<T>(Func<IEnumerable<T>> testCases, Action<T> action)
        {
            //throw new NotImplementedException();
        }

        protected void it(string description, Action action)
        {
            _visitor.visitIt(description, action);
        }

    }
}
