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
        void AddChildTest(INJasmineBuildResult test);
        void AddIgnoreReason(string ignoreReason);
        void AddCategory(string category);
        List<INJasmineBuildResult> Children { get; }
        string ReasonIgnored { get; }
        List<string> Categories { get; }
    }

    public class NJasmineBuilder : INJasmineBuildResult
    {
        string _ignoreReason;
        List<INJasmineBuildResult> _children = new List<INJasmineBuildResult>(); 
        List<string> _categories = new List<string>();
        Func<Test> _creationStrategy; 

        public NJasmineBuilder(Func<Test> factory)
        {
            _creationStrategy = factory;
        }

        public Test GetNUnitResult()
        {
            return BuildTest.GetNUnitResultInternal(this, this._creationStrategy);
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

        public List<INJasmineBuildResult> Children
        {
            get { return _children; }
        }

        public string ReasonIgnored
        {
            get { return _ignoreReason; }
        }

        public List<string> Categories
        {
            get { return _categories; }
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
