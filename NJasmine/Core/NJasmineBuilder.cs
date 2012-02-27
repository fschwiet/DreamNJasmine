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
        string Shortname { get; set; }
        string FullName { get; set; }
        string MultilineName { get; set; }
        string ReasonIgnored { get; }
        List<INJasmineBuildResult> Children { get; }
        List<string> Categories { get; }
        void AddChildTest(INJasmineBuildResult test);
        void AddIgnoreReason(string ignoreReason);
        void AddCategory(string category);
    }

    public class NJasmineBuilder : INJasmineBuildResult
    {
        readonly NativeTest _nativeTest;

        public NJasmineBuilder(NativeTest nativeTest)
        {
            _nativeTest = nativeTest;
            Children = new List<INJasmineBuildResult>();
            Categories = new List<string>();
        }

        public Test GetNUnitResult()
        {
            return BuildTest.GetNUnitResultInternal(this, _nativeTest);
        }

        public string Shortname { get; set; }
        public string FullName { get; set; }
        public string MultilineName { get; set; }

        public string ReasonIgnored { get; private set; }

        public List<INJasmineBuildResult> Children { get; private set; }
        public List<string> Categories { get; private set; }

        public void AddChildTest(INJasmineBuildResult test)
        {
            Children.Add(test);
        }

        public void AddCategory(string category)
        {
            Categories.Add(category);
        }

        public void AddIgnoreReason(string ignoreReason)
        {
            if (String.IsNullOrEmpty(ReasonIgnored))
                ReasonIgnored = ignoreReason;
            else
                ReasonIgnored = ReasonIgnored + ", " + ignoreReason;
        }    
    }
}
