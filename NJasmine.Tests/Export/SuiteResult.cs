using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class SuiteResult
    {
        private readonly string _name;
        private readonly XElement _xml;

        public SuiteResult(XElement xml)
        {
            _xml = xml;

            if (xml.Attribute("name") != null)
                _name = xml.Attribute("name").Value;
            else
                _name = "unknown";
        }

        public SuiteResult thatsInconclusive()
        {
            return thatHasResult("Inconclusive");
        }

        public SuiteResult thatSucceeds()
        {
            return thatHasResult("Success");
        }

        private SuiteResult thatHasResult(string inconclusive)
        {
            var result = _xml.Attribute("result").Value;

            Assert.AreEqual(inconclusive, result, String.Format("Expected suite named {0} to be {1}.", _name, inconclusive));

            return this;
        }

        public SuiteResult thatHasNoResults()
        {
            var results = _xml.Descendants("results");

            Assert.AreEqual(0, results.Count(), "Expected suite " + _name + " to not have any results.");

            return this;
        }

        public TestResult ShouldHaveTest(string name)
        {
            var tests = _xml.Descendants("test-case").Where(e => e.Attribute("name") != null && e.Attribute("name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            Assert.AreEqual(1, tests.Count(),
                String.Format("Expected test not found in suite {0}, expected test named {1}", _name, name));

            return new TestResult();
        }

        public void withCategories(params string[] categories)
        {
            var actualCategories = new List<string>();

            var categoriesXml = _xml.Elements("categories").Single();

            if (categoriesXml != null)
            {
                foreach(var category in categoriesXml.Elements("category").Select(c => c.Attribute("name").Value))
                {
                    actualCategories.Add(category);
                }
            }

            categories = categories.OrderBy(c => c).ToArray();
            actualCategories = actualCategories.OrderBy(c => c).ToList();

            Assert.That(categories, Is.EquivalentTo(actualCategories));
        }
    }
}