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

        public GlobalSetupOwner GlobalSetupOwner = new GlobalSetupOwner();
        
        public Dictionary<string, TestContext> Contexts = new Dictionary<string, TestContext>();
        public Dictionary<Func<SpecificationFixture>, Dictionary<TestPosition, GenericNativeTest>> TestAt = 
            new Dictionary<Func<SpecificationFixture>, Dictionary<TestPosition, GenericNativeTest>>(); 

        public void SetRoot(INativeTest test)
        {
        }

        public INativeTest ForSuite(FixtureContext fixtureContext, TestContext testContext)
        {
            var result = new GenericNativeTest(testContext.Name);
            RecordTestAt(fixtureContext.FixtureFactory, testContext.Position, result);
            return result;
        }

        public INativeTest ForTest(FixtureContext fixtureContext, TestContext testContext)
        {
            Contexts[testContext.Name.FullName] = testContext;
            var result = new GenericNativeTest(testContext.Name);
            RecordTestAt(fixtureContext.FixtureFactory, testContext.Position, result);
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
            GlobalSetupOwner.Dispose();
        }

        public string GetIgnoreReason(string testName, string explicitlyIncluding)
        {
            List<GenericNativeTest> selfAndAncestors = new List<GenericNativeTest>();

            var context = Contexts[testName];
            var fixtureBuilder = context.FixtureContext.FixtureFactory;
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
                if (test.ReasonIgnored != null && !explicitlyIncluding.StartsWith(test.Name.FullName))
                    return test.ReasonIgnored;
            }

            return null;
        }
    }
}
