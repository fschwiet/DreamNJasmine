using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NJasmine.Core
{
    public class ExplicitAttributeReader
    {
        public static string GetFor(Type type)
        {
            var explicitAttributes = type.GetCustomAttributes(false)
                .Where(a => a.GetType().FullName.Contains("NUnit") && a.GetType().Name == "ExplicitAttribute");

            if (explicitAttributes.Count() == 0)
                return null;

            string explicitReason = null;
            foreach (var explicitAttribute in explicitAttributes)
            {
                var reasonProperty = explicitAttribute.GetType().GetProperty("Reason",
                    BindingFlags.Public | BindingFlags.Instance);

                if (reasonProperty != null)
                {
                    var reason = (string)reasonProperty.GetGetMethod(false).Invoke(explicitAttribute, new object[0]);

                    if (!string.IsNullOrEmpty(reason))
                    {
                        explicitReason = reason;
                    }
                }
            }

            if (explicitReason == null)
                return type.Name + " has attribute " + explicitAttributes.First().GetType().Name + ".";
            else
                return explicitReason;
        }
    }
}
