using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.GlobalSetup;
using NUnit.Core;

namespace NJasmine.Core
{
    public interface INJasmineBuildResult : INJasmineNameable
    {
        Test GetNUnitResult();
        void TurnIntoAFailingSuite(Exception exception, TestPosition position);
        string Shortname { get; set; }
        string FullName { get; set; }
        string MultilineName { get; set; }
        void AddChildTest(INJasmineBuildResult test);
        void AddIgnoreReason(string ignoreReason);
        void AddCategory(string category);
    }

    class NJasmineBuildResult : INJasmineBuildResult
    {
        string _ignoreReason;
        List<INJasmineBuildResult> _children = new List<INJasmineBuildResult>(); 
        List<string> _categories = new List<string>();
        Func<Test> _creationStrategy; 

        NJasmineBuildResult()
        {
        }

        public static NJasmineBuildResult ForUnimplementedTest(TestPosition position)
        {
            var result = new NJasmineBuildResult();
            result._creationStrategy = () => new NJasmineUnimplementedTestMethod(position);
            return result;
        }

        public static NJasmineBuildResult ForSuite(TestPosition position, Action onetimeCleanup)
        {
            var result = new NJasmineBuildResult();
            result._creationStrategy = () => new NJasmineTestSuiteNUnit("hi", "there", onetimeCleanup, position);
            return result;
        }

        public static NJasmineBuildResult ForTest(Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager)
        {
            var result = new NJasmineBuildResult();
            result._creationStrategy = () => new NJasmineTestMethod(fixtureFactory, position, globalSetupManager);
            return result;
        }

        public void TurnIntoAFailingSuite(Exception exception, TestPosition position)
        {
            _creationStrategy = () => new NJasmineInvalidTestSuite(exception, position);
            _children = new List<INJasmineBuildResult>();
        }

        public Test GetNUnitResult()
        {
            Test result;

            result = _creationStrategy();

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

        public string Shortname { get; set; }
        public string FullName { get; set; }
        public string MultilineName { get; set; }

        public void AddChildTest(INJasmineBuildResult test)
        {
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
}
