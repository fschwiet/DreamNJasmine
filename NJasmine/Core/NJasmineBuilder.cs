using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.Core
{
    public class NJasmineBuilder : INJasmineNameable
    {
        public readonly NativeTest NativeTest;

        public NJasmineBuilder(NativeTest nativeTest)
        {
            NativeTest = nativeTest;
            Children = new List<NJasmineBuilder>();
            Categories = new List<string>();
        }

        public string Shortname { get; set; }
        public string FullName { get; set; }
        public string MultilineName { get; set; }

        public string ReasonIgnored { get; private set; }

        public List<NJasmineBuilder> Children { get; private set; }
        public List<string> Categories { get; private set; }

        public void AddChildTest(NJasmineBuilder test)
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
