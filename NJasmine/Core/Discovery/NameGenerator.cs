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
            test.Shortname = testShortName;
            test.FullName = parentTest.FullName + ", " + testShortName;
            test.MultilineName = parentTest.MultilineName + ",\n" + testShortName;

            IncrementTestNameUntilItsNot(test, IsReserved);

            _globallyAccumulatedTestNames[test.FullName] = NameIs.Available;
        }

        public void NameTest(string testShortName, TestBuilder parentTest, TestBuilder test)
        {
            test.Shortname = testShortName;
            test.FullName = parentTest.FullName + ", " + testShortName;
            test.MultilineName = parentTest.MultilineName + ",\n" + testShortName;

            IncrementTestNameUntilItsNot(test, name => _globallyAccumulatedTestNames.ContainsKey(name));
            _globallyAccumulatedTestNames[test.FullName] = NameIs.Reserved;
        }

        public void ReserveName(TestBuilder test)
        {
            IncrementTestNameUntilItsNot(test, name => _globallyAccumulatedTestNames.ContainsKey(name) && _globallyAccumulatedTestNames[name] == NameIs.Reserved);
            _globallyAccumulatedTestNames[test.FullName] = NameIs.Reserved;
        }

        private void IncrementTestNameUntilItsNot(TestBuilder test, Func<string, bool> condition)
        {
            var name = test.FullName;

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


                test.Shortname = test.Shortname + suffix;
                test.FullName = test.FullName + suffix;
                test.MultilineName = test.MultilineName + suffix;
            }
        }
    }
}
