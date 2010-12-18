﻿using System;
using System.Collections.Generic;

namespace NJasmine.Core.FixtureVisitor
{
    public class TerminalVisitor : INJasmineFixtureVisitor
    {
        readonly SpecMethod _specMethod;
        readonly INJasmineFixturePositionVisitor _originalVisitor;

        public TerminalVisitor(SpecMethod specMethod, INJasmineFixturePositionVisitor originalVisitor)
        {
            _specMethod = specMethod;
            _originalVisitor = originalVisitor;
        }

        public void visitDescribe(string description, Action action)
        {
            throw DontException(SpecMethod.describe);
        }

        public void visitBeforeEach(Action action)
        {
            throw DontException(SpecMethod.beforeEach);
        }

        public void visitAfterEach(Action action)
        {
            throw DontException(SpecMethod.afterEach);
        }

        public void visitIt(string description, Action action)
        {
            throw DontException(SpecMethod.it);
        }

        public TFixture visitImportNUnit<TFixture>() where TFixture: class, new()
        {
            throw DontException(SpecMethod.importNUnit);
        }

        public TArranged visitArrange<TArranged>(string description, IEnumerable<Func<TArranged>> factories)
        {
            return _originalVisitor.visitArrange(description, factories, null);
        }

        InvalidOperationException DontException(SpecMethod innerSpecMethod)
        {
            return new InvalidOperationException("Called " + innerSpecMethod + "() within " + _specMethod + "().");
        }
    }
}
