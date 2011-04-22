using System.Collections.Generic;
using NUnit.Core;

namespace NJasmine.Core.Discovery
{
    class NameGenerator
    {
        enum NameIs
        {
            Available,
            Reserved
        }

        readonly Dictionary<string, NameIs> _globallyAccumulatedTestNames = new Dictionary<string, NameIs>();

        public void NameTest(Test parentTest, string testShortName, Test test)
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

            if (_globallyAccumulatedTestNames.ContainsKey(name))
            {
                var nextIndex = 1;
                string suffix;
                string nextName;

                do
                {
                    suffix = "`" + ++nextIndex;
                    nextName = name + suffix;
                } while (_globallyAccumulatedTestNames.ContainsKey(nextName));


                test.TestName.Name = test.TestName.Name + suffix;
                test.TestName.FullName = test.TestName.FullName + suffix;
            }

            _globallyAccumulatedTestNames[test.TestName.FullName] = NameIs.Reserved;
        }
    }
}
