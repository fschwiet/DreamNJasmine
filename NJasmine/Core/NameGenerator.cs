using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    class NameGenerator
    {
        readonly List<string> _globallyAccumulatedTestNames = new List<string>();

        public void NameTest(NJasmineTestSuite parentTest, string testShortName, Test test)
        {
            NameTest(parentTest.TestName.FullName, testShortName, test);
        }

        public void NameTest(string parentFullName, string testShortName, Test test)
        {
            test.TestName.FullName = parentFullName + ", " + testShortName;
            test.TestName.Name = testShortName;
            MakeNameUnique(test);
        }

        public void MakeNameUnique(Test test)
        {
            var name = test.TestName.FullName;

            if (_globallyAccumulatedTestNames.Contains(name))
            {
                var nextIndex = 1;
                string suffix;
                string nextName;

                do
                {
                    suffix = "`" + ++nextIndex;
                    nextName = name + suffix;
                } while (_globallyAccumulatedTestNames.Contains(nextName));


                test.TestName.Name = test.TestName.Name + suffix;
                test.TestName.FullName = test.TestName.FullName + suffix;
            }

            _globallyAccumulatedTestNames.Add(test.TestName.FullName);
        }
    }
}
