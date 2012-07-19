using System;
using System.Collections.Generic;

namespace NJasmine.Core.Discovery
{
    public class NameReservations
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

        public TestName GetSharedTestName(string testShortName, TestName parentTestName)
        {
            var result = GetChildTestName(testShortName, parentTestName);

            IncrementTestNameUntilItsNot(result, IsReserved);

            _globallyAccumulatedTestNames[result.FullName] = NameIs.Available;

            return result;
        }

        public TestName GetReservedTestName(string testShortName, TestName parentTestName)
        {
            var testName = GetChildTestName(testShortName, parentTestName);

            IncrementTestNameUntilItsNot(testName, name => _globallyAccumulatedTestNames.ContainsKey(name));
            _globallyAccumulatedTestNames[testName.FullName] = NameIs.Reserved;

            return testName;
        }

        static TestName GetChildTestName(string testShortName, TestName parentTestName)
        {
            var testName = new TestName();

            testName.Shortname = testShortName;
            testName.FullName = parentTestName.FullName + ", " + testShortName;
            testName.MultilineName = parentTestName.MultilineName + ",\n" + testShortName;
            return testName;
        }

        public TestName GetReservedNameLike(TestName testName)
        {
            var result = new TestName();
            result.Shortname = testName.Shortname;
            result.FullName = testName.FullName;
            result.MultilineName = testName.MultilineName;

            IncrementTestNameUntilItsNot(result, name => _globallyAccumulatedTestNames.ContainsKey(name) && _globallyAccumulatedTestNames[name] == NameIs.Reserved);
            _globallyAccumulatedTestNames[result.FullName] = NameIs.Reserved;

            return result;
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
