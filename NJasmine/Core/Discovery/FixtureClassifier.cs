using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.Core.Discovery
{
    public class FixtureClassifier
    {
        public static bool IsTypeSpecification(Type type)
        {
            if (!type.IsSubclassOf(typeof (SpecificationFixture)))
                return false;

            if (!(type.IsPublic || type.IsNestedPublic))
                return false;

            if (type.GetConstructor(new Type[0]) == null) // expression really can be false, don't believe Resharper
                return false;

            return true;
        }
    }
}
