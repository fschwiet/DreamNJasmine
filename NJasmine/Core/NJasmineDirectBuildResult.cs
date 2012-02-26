using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public interface INJasmineBuildResult : INJasmineNameable
    {
        Test GetNUnitResult();
        void TurnIntoAFailingSuite(Exception exception);
        bool IsSuite();
        string Shortname { get; set; }
        string FullName { get; set; }
        string MultilineName { get; set; }
        void AddChildTest(INJasmineBuildResult test);
        void AddIgnoreReason(string ignoreReason);
        void AddCategory(string category);
    }

    class NJasmineBuildResult : INJasmineBuildResult
    {
        readonly TestPosition _position;
        bool _isSuite;
        bool _unimplementedTest;
        Exception _failureException;
        string _ignoreReason;
        List<INJasmineBuildResult> _children = new List<INJasmineBuildResult>(); 
        List<string> _categories = new List<string>();
        Action _onetimeCleanup;

        public NJasmineBuildResult(bool isSuite, TestPosition position, Action onetimeCleanup)
        {
            _position = position;
            _isSuite = isSuite;
            _onetimeCleanup = onetimeCleanup;
        }

        public static NJasmineBuildResult ForUnimplementedTest(TestPosition position)
        {
            var result = new NJasmineBuildResult(false, position, () => {});
            result._unimplementedTest = true;
            return result;
        }

        public static NJasmineBuildResult ForSuite(TestPosition position, Action onetimeCleanup)
        {
            var result = new NJasmineBuildResult(true, position, onetimeCleanup);
            result._onetimeCleanup = onetimeCleanup;
            return result;
        }

        public Test GetNUnitResult()
        {
            Test result;

            if (_failureException != null)
            {
                result = new NJasmineInvalidTestSuite(_failureException, _position);
            } 
            else if (_unimplementedTest)
            {
                result = new NJasmineUnimplementedTestMethod(_position);
            } 
            else if (IsSuite())
            {
                result = new NJasmineTestSuiteNUnit("hi", "there", _onetimeCleanup, _position);
            } 
            else
            {
                throw new NotImplementedException();
            }

            ApplyPropertiesToTest(result);

            foreach(var childTest in _children)
            {
                (result as TestSuite).Add(childTest.GetNUnitResult());
            }

            return result;
        }

        void ApplyPropertiesToTest(Test result)
        {
            result.TestName.Name = Shortname;
            result.TestName.FullName = FullName;
            result.SetMultilineName(MultilineName);

            if (_ignoreReason != null)
            {
                result.RunState = RunState.Explicit;
                result.IgnoreReason = _ignoreReason;
            }

            foreach (var category in _categories)
                result.Categories.Add(category);
        }

        public void TurnIntoAFailingSuite(Exception exception)
        {
            AssertIsSuite();

            _failureException = exception;
        }

        void AssertIsSuite()
        {
            if (!_isSuite)
                throw new InvalidProgramException();
        }

        public bool IsSuite()
        {
            return _isSuite;
        }

        public string Shortname { get; set; }
        public string FullName { get; set; }
        public string MultilineName { get; set; }

        public void AddChildTest(INJasmineBuildResult test)
        {
            AssertIsSuite();
            _children.Add(test);
        }

        public void AddIgnoreReason(string ignoreReason)
        {
            if (String.IsNullOrEmpty(_ignoreReason))
                _ignoreReason = ignoreReason;
            else
                _ignoreReason = _ignoreReason + ", " + ignoreReason;
        }

        public void AddCategory(string category)
        {
            _categories.Add(category);
        }

        string INJasmineNameable.Shortname
        {
            get { return Shortname; }
            set { Shortname = value; }
        }

        string INJasmineNameable.FullName
        {
            get { return FullName; }
            set { FullName = value; }
        }

        string INJasmineNameable.MultilineName
        {
            get { return MultilineName; }
            set { MultilineName = value; }
        }
    }

    public class NJasmineDirectBuildResult : INJasmineBuildResult
    {
        TestPosition _position;
        private Test _test;

        public NJasmineDirectBuildResult(Test test, TestPosition position)
        {
            _test = test;
        }

        public Test GetNUnitResult()
        {
            return _test;
        }

        public bool IsSuite()
        {
            return _test.IsSuite;
        }

        public string Shortname
        {
            get { return _test.TestName.Name; }
            set { _test.TestName.Name = value; }
        }

        public string FullName
        {
            get { return _test.TestName.FullName; }
            set { _test.TestName.FullName = value; }
        }

        public string MultilineName
        {
            get { return TestExtensions.GetMultilineName(_test); }
            set { TestExtensions.SetMultilineName(_test, value); }
        }

        public void AddChildTest(INJasmineBuildResult test)
        {
            (_test as TestSuite).Add(test.GetNUnitResult());
        }

        public void AddIgnoreReason(string ignoreReason)
        {
            _test.RunState = RunState.Explicit;
            
            if (String.IsNullOrEmpty(_test.IgnoreReason))
                _test.IgnoreReason = ignoreReason;
            else
                _test.IgnoreReason = _test.IgnoreReason + ", " + ignoreReason;
        }

        public void AddCategory(string category)
        {
            NUnitFrameworkUtil.ApplyCategoryToTest(category, _test);
        }

        public void TurnIntoAFailingSuite(Exception exception)
        {
            Test test = new NJasmineInvalidTestSuite(exception, _position);
            test.TestName.Name = Shortname;
            test.TestName.FullName = FullName;
            test.SetMultilineName(MultilineName);

            _test = test;
        }
    }
}
