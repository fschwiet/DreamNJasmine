using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core.Discovery;


namespace NJasmine.Core
{
    public class TestBuilder
    {
        readonly INativeTest _nativeTest;

        public TestBuilder(INativeTest nativeTest, TestName name = null)
        {
            _nativeTest = nativeTest;
            Children = new List<TestBuilder>();
            Categories = new List<string>();
            Name = name ?? new TestName();
        }

        public TestName Name { get; set; }

        public string ReasonIgnored { get; private set; }

        public List<TestBuilder> Children { get; private set; }
        public List<string> Categories { get; private set; }

        public void AddChildTest(TestBuilder test)
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

        public INativeTest GetUnderlyingTest()
        {
            return _nativeTest;
        }

        public static TestBuilder RunSuiteAction(TestContext testContext1, SharedContext sharedContext, Action action, bool isOuterScopeOfSpecification, TestBuilder resultBuilder)
        {
            var builder = new DiscoveryVisitor(resultBuilder, sharedContext, testContext1.GlobalSetupManager);

            var exception = sharedContext.RunActionWithVisitor(testContext1.Position.GetFirstChildPosition(), action, builder);

            if (exception == null)
            {
                builder.VisitAccumulatedTests(v => resultBuilder.AddChildTest(v));
            }
            else
            {
                var testContext = new TestContext()
                {
                    Name = sharedContext.NameReservations.GetReservedNameLike(resultBuilder.Name),
                    Position = testContext1.Position,
                    GlobalSetupManager = testContext1.GlobalSetupManager
                };

                var failingSuiteAsTest = new TestBuilder(sharedContext.NativeTestFactory.ForFailingSuite(testContext, exception), testContext.Name);

                if (isOuterScopeOfSpecification)
                {
                    resultBuilder.AddChildTest(failingSuiteAsTest);
                }
                else
                {
                    return failingSuiteAsTest;
                }
            }
            return resultBuilder;
        }
    }
}
