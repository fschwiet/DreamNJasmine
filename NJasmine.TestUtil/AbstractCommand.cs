using System.IO;
using System.Web.Script.Serialization;

namespace NJasmineTestLoader
{
    public abstract class AbstractCommand
    {
        public abstract string Name { get; }
        public abstract void LoadArgs(string[] args);
        public abstract int Run();
        public abstract void WriteExpectedArguments(TextWriter tw, string exeName);
        
        public virtual void Trace(TextWriter tw)
        {
            tw.WriteLine(this.GetType().Name + " = " + new JavaScriptSerializer().Serialize(this));
        }
    }
}