using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core.Discovery;


namespace NJasmine.Core
{
    public class TestBuilder
    {
        private string ReasonIgnored;
        readonly INativeTest _nativeTest;

        public TestBuilder(INativeTest nativeTest)
        {
            _nativeTest = nativeTest;
        }

        public void AddChildTest(TestBuilder test)
        {
            _nativeTest.AddChild(test);
        }

        public void AddCategory(string category)
        {
            var invalidReason = Validate.CheckForCategoryError(category);
            if (invalidReason != null)
            {
                _nativeTest.MarkTestInvalid(invalidReason);
            }
            else
            {
                _nativeTest.AddCategory(category);
            }
        }

        public void AddIgnoreReason(string ignoreReason)
        {
            if (String.IsNullOrEmpty(ReasonIgnored))
                ReasonIgnored = ignoreReason;
            else
                ReasonIgnored = ReasonIgnored + ", " + ignoreReason;

            _nativeTest.MarkTestIgnored(ReasonIgnored);
        }

        public INativeTest GetUnderlyingTest()
        {
            return _nativeTest;
        }

        public static TestBuilder BuildSuiteForTextContext(SharedContext sharedContext, TestContext testContext1, Action invoke, bool isRootSuite, string explicitReason = null)
        {
            var resultBuilder = new TestBuilder(sharedContext.NativeTestFactory.ForSuite(testContext1));

            if (explicitReason != null)
                resultBuilder.AddIgnoreReason(explicitReason);

            var builder = new DiscoveryVisitor(resultBuilder, sharedContext, testContext1.GlobalSetupManager);

            var exception = sharedContext.RunActionWithVisitor(testContext1.Position.GetFirstChildPosition(), invoke, builder);

            if (exception == null)
            {
                builder.VisitAccumulatedTests(resultBuilder.AddChildTest);
            }
            else
            {
                var testContext = new TestContext()
                {
                    Name = sharedContext.NameReservations.GetReservedNameLike(resultBuilder.GetUnderlyingTest().Name),
                    Position = testContext1.Position,
                    GlobalSetupManager = testContext1.GlobalSetupManager
                };

                var failingSuiteAsTest = new TestBuilder(sharedContext.NativeTestFactory.ForTest(sharedContext, testContext));
                failingSuiteAsTest.GetUnderlyingTest().MarkTestFailed(exception);

                if (isRootSuite)
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
