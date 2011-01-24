using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core;
using NUnit.Core;
using NUnit.Framework;
using Should.Fluent;
using Should.Fluent.Model;
using Assert = Should.Core.Assertions.Assert;

namespace NJasmine
{
    public abstract class NJasmineFixture : SkeleFixture, IArrangeContext
    {
        public class VisitorChangedContext : IDisposable
        {
            Action _action;

            public VisitorChangedContext(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                if (_action != null)
                {
                    _action();
                    _action = null;
                }
            }
        }

        public void describe(string description)
        {
            describe(description, null);
        }

        public void describe(string description, Action action)
        {
            _visitor.visitDescribe(description, action);
        }

        public void beforeEach(Action action)
        {
            _visitor.visitBeforeEach(action);
        }

        public void afterEach(Action action)
        {
            _visitor.visitAfterEach(action);
        }

        public void it(string description)
        {
            _visitor.visitIt(description, null);
        }

        public void it(string description, Action action)
        {
            _visitor.visitIt(description, action);
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

            return _visitor.visitArrange<TArranged>(null, new [] {factory});
        }

        public TArranged arrange<TArranged>(Func<TArranged> factory)
        {
            return _visitor.visitArrange<TArranged>(null, new[] { factory });
        }

        public void arrange(Action action)
        {
            arrange(null, action);
        }

        public void arrange(string description, params Action[] actions)
        {
            List<Func<object>> factories = new List<Func<object>>();

            foreach(var actionCursor in actions)
            {
                var action = actionCursor;

                Func<object> nilFactory = delegate
                {
                    action();
                    return null;
                };

                factories.Add(nilFactory);
            }

            _visitor.visitArrange<object>(description, factories.ToArray());
        }
    }
}
