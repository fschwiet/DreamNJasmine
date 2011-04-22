using System;
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

        bool IsReserved(string name)
        {
            if (!_globallyAccumulatedTestNames.ContainsKey(name))
                return false;

            return _globallyAccumulatedTestNames[name] == NameIs.Reserved;
        }

        public void NameFork(string parentFullName, string testShortName, Test test, out bool reusedName)
        {
            test.TestName.FullName = parentFullName + ", " + testShortName;
            test.TestName.Name = testShortName;

            IncrementTestNameUntilItsNot(test, IsReserved);

            reusedName = _globallyAccumulatedTestNames.ContainsKey(test.TestName.FullName);

            _globallyAccumulatedTestNames[test.TestName.FullName] = NameIs.Available;
        }

        public void NameTest(string parentFullName, string testShortName, Test test)
        {
            test.TestName.FullName = parentFullName + ", " + testShortName;
            test.TestName.Name = testShortName;

            MakeNameUnique(test);
        }

        public void MakeNameUnique(Test test)
        {
            IncrementTestNameUntilItsNot(test, name => _globallyAccumulatedTestNames.ContainsKey(name));
            _globallyAccumulatedTestNames[test.TestName.FullName] = NameIs.Reserved;
        }

        private void IncrementTestNameUntilItsNot(Test test, Func<string, bool> condition)
        {
            var name = test.TestName.FullName;

            if (condition(name))
            {
                var nextIndex = 1;
                string suffix;
                string nextName;

                do
                {
                    suffix = "`" + ++nextIndex;
                    nextName = name + suffix;
                } while (condition(nextName));


                test.TestName.Name = test.TestName.Name + suffix;
                test.TestName.FullName = test.TestName.FullName + suffix;
            }
        }
    }
}
