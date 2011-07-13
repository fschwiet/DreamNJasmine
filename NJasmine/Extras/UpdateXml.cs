using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NJasmine.Extras
{

    public class UpdateXml
    {
        public interface IXmlTransform
        {
            object Get(string xpath);
            object Set(string xpath, object value);
            void ForEach(string xpath, Action continuation);
        }

        public void Run(string filename, Action<IXmlTransform> xmlTransform)
        {
            var doc = new XmlDocument();
            var currentNamespace = new XmlNamespaceManager(doc.NameTable);
            
            var currentNode = doc;

            doc.Load(filename);

            var transform = new XmlTransformer();

            xmlTransform

            doc.Save(filename);

        }

        private class XmlTransformer : IXmlTransform
        {
            public object Get(string xpath)
            {
                throw new NotImplementedException();
            }

            public object Set(string xpath, object value)
            {
                throw new NotImplementedException();
            }

            public void ForEach(string xpath, Action continuation)
            {
                throw new NotImplementedException();
            }
        }
    }
}
