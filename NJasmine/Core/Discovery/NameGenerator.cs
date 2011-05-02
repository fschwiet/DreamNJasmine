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

        public void NameFork(string testShortName, INJasmineTest parentTest, INJasmineTest test, out bool reusedName)
        {
            test.TestName.Name = testShortName;
            test.TestName.FullName = parentTest.TestName.FullName + ", " + testShortName;
            test.SetMultilineName(parentTest.GetMultilineName() + ",\n" + testShortName);

            IncrementTestNameUntilItsNot(test, IsReserved);

            reusedName = _globallyAccumulatedTestNames.ContainsKey(test.TestName.FullName);

            _globallyAccumulatedTestNames[test.TestName.FullName] = NameIs.Available;
        }

        public void NameTest(string testShortName, INJasmineTest parentTest, INJasmineTest test)
        {
            test.TestName.Name = testShortName;
            test.TestName.FullName = parentTest.TestName.FullName + ", " + testShortName;
            test.SetMultilineName(parentTest.GetMultilineName() + ",\n" + testShortName);

            MakeNameUnique(test);
        }

        public void MakeNameUnique(INJasmineTest test)
        {
            IncrementTestNameUntilItsNot(test, name => _globallyAccumulatedTestNames.ContainsKey(name));
            _globallyAccumulatedTestNames[test.TestName.FullName] = NameIs.Reserved;
        }

        private void IncrementTestNameUntilItsNot(INJasmineTest test, Func<string, bool> condition)
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
                test.SetMultilineName(test.GetMultilineName() + suffix);
            }
        }
    }
}
