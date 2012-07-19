using System;
using System.Collections.Generic;

namespace NJasmine.Core.Discovery
{
    public class NameGenerator
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

        public void NameFork(string testShortName, TestBuilder parentTest, TestBuilder test)
        {
            test.Name.Shortname = testShortName;
            test.Name.FullName = parentTest.Name.FullName + ", " + testShortName;
            test.Name.MultilineName = parentTest.Name.MultilineName + ",\n" + testShortName;

            IncrementTestNameUntilItsNot(test.Name, IsReserved);

            _globallyAccumulatedTestNames[test.Name.FullName] = NameIs.Available;
        }

        public TestName NameTest(string testShortName, TestBuilder parentTest)
        {
            var testName = new TestName();

            testName.Shortname = testShortName;
            testName.FullName = parentTest.Name.FullName + ", " + testShortName;
            testName.MultilineName = parentTest.Name.MultilineName + ",\n" + testShortName;

            IncrementTestNameUntilItsNot(testName, name => _globallyAccumulatedTestNames.ContainsKey(name));
            _globallyAccumulatedTestNames[testName.FullName] = NameIs.Reserved;

            return testName;
        }

        public void ReserveName(TestName testName)
        {
            IncrementTestNameUntilItsNot(testName, name => _globallyAccumulatedTestNames.ContainsKey(name) && _globallyAccumulatedTestNames[name] == NameIs.Reserved);
            _globallyAccumulatedTestNames[testName.FullName] = NameIs.Reserved;
        }

        private void IncrementTestNameUntilItsNot(TestName testName, Func<string, bool> condition)
        {
            var name = testName.FullName;

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


                testName.Shortname = testName.Shortname + suffix;
                testName.FullName = testName.FullName + suffix;
                testName.MultilineName = testName.MultilineName + suffix;
            }
        }
    }
}
