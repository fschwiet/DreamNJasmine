using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.Core
{
    public interface INJasmineFixturePositionVisitor
    {
        void visitDescribe(string description, Action action, TestPosition position);
        void visitBeforeEach(Action action, TestPosition position);
        void visitAfterEach(Action action, TestPosition position);
        void visitIt(string description, Action action, TestPosition position);
    }
}
