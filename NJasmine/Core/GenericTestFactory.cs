﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;
using NJasmine.Core.NativeWrappers;

namespace NJasmine.Core
{
    public class GenericTestFactory : INativeTestFactory, IDisposable
    {
        public IEnumerable<string> Names
        {
            get { return Contexts.Select(kvp => kvp.Key); }
        }

        public GlobalSetupManager GlobalSetupManager;

        public Dictionary<string, Func<SpecificationFixture>> FixtureBuilders =
            new Dictionary<string, Func<SpecificationFixture>>(); 
        public Dictionary<string, TestContext> Contexts = new Dictionary<string, TestContext>();
        public Dictionary<Func<SpecificationFixture>, Dictionary<TestPosition, GenericNativeTest>> TestAt = 
            new Dictionary<Func<SpecificationFixture>, Dictionary<TestPosition, GenericNativeTest>>(); 

        public void SetRoot(INativeTest test)
        {
        }

        public INativeTest ForSuite(SharedContext sharedContext, TestContext testContext)
        {
            var result = new GenericNativeTest(testContext.Name);
            RecordTestAt(sharedContext.FixtureFactory, testContext.Position, result);
            return result;
        }

        public INativeTest ForTest(SharedContext sharedContext, TestContext testContext)
        {
            FixtureBuilders[testContext.Name.FullName] = sharedContext.FixtureFactory;
            Contexts[testContext.Name.FullName] = testContext;
            var result = new GenericNativeTest(testContext.Name);
            RecordTestAt(sharedContext.FixtureFactory, testContext.Position, result);
            return result;
        }

        private void RecordTestAt(Func<SpecificationFixture> fixtureFactory, TestPosition testPosition, GenericNativeTest result)
        {
            if (!TestAt.ContainsKey(fixtureFactory))
                TestAt[fixtureFactory] = new Dictionary<TestPosition, GenericNativeTest>();
            
            TestAt[fixtureFactory][testPosition] = result;
        }

        public void Dispose()
        {
            if (GlobalSetupManager != null)
            {
                GlobalSetupManager.Close();
                GlobalSetupManager = null;
            }
        }

        public string GetIgnoreReason(string name)
        {
            List<GenericNativeTest> selfAndAncestors = new List<GenericNativeTest>();

            var fixtureBuilder = FixtureBuilders[name];
            var context = Contexts[name];
            var position = context.Position;
            
            while(true)
            {
                selfAndAncestors.Add(TestAt[fixtureBuilder][position]);

                if (position.Coordinates.Count() == 0)
                    break;

                position = position.Parent;
            }

            foreach(var test in selfAndAncestors)
            {
                if (test.ReasonIgnored != null)
                    return test.ReasonIgnored;
            }

            return null;
        }
    }
}
