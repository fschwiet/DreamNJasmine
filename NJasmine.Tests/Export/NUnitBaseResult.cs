using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class NUnitBaseResult
    {
        protected string _name;
        private readonly string _descriptiveNameOfResultType;
        protected XElement _xml;

        public NUnitBaseResult(string descriptiveNameOfResultType, XElement xml)
        {
            _descriptiveNameOfResultType = descriptiveNameOfResultType;
            _xml = xml;

            if (xml.Attribute("name") != null)
                _name = xml.Attribute("name").Value;
            else
                _name = "unknown";
        }

        protected void thatHasResult(string inconclusive)
        {
            var result = GetResult();

            Assert.AreEqual(inconclusive, result, String.Format("Expected {0} named {1} to be {2}.", _descriptiveNameOfResultType, _name, inconclusive));
        }

        protected string GetResult()
        {
            return _xml.Attribute("result").Value;
        }

        protected TResult withCategories<TResult>(params string[] categories) where TResult : NUnitBaseResult
        {
            var actualCategories = new List<string>();

            var categoriesXml = _xml.Elements("categories");

            if (categoriesXml != null)
            {
                foreach(var category in categoriesXml.Elements("category").Select(c => c.Attribute("name").Value))
                {
                    actualCategories.Add(category);
                }
            }

            categories = categories.OrderBy(c => c).ToArray();
            actualCategories = actualCategories.OrderBy(c => c).ToList();

            Assert.That(categories, Is.EquivalentTo(actualCategories),
                String.Format("Expected {0} named {1} to have categories {2}, actually had {3}",
                    _descriptiveNameOfResultType, 
                    _name, 
                    string.Join(",", categories), 
                    string.Join(",", actualCategories)));

            return this as TResult;
        }
    }
}