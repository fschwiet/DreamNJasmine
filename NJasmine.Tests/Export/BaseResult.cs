using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class BaseResult
    {
        protected string _name;
        private readonly string _typeName;
        protected XElement _xml;

        public BaseResult(string typeName, XElement xml)
        {
            _typeName = typeName;
            _xml = xml;

            if (xml.Attribute("name") != null)
                _name = xml.Attribute("name").Value;
            else
                _name = "unknown";
        }

        protected void thatHasResult(string inconclusive)
        {
            var result = GetResult();

            Assert.AreEqual(inconclusive, result, String.Format("Expected {0} named {1} to be {2}.", _typeName, _name, inconclusive));
        }

        protected string GetResult()
        {
            return _xml.Attribute("result").Value;
        }

        public void withCategories(params string[] categories)
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
                    _typeName, 
                    _name, 
                    string.Join(",", categories), 
                    string.Join(",", actualCategories)));
        }
    }
}