using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.Core.GlobalSetup
{
    public class GlobalSetupOwner : IDisposable
    {
        private Dictionary<Type, GlobalSetupManager> _setupManagers = new Dictionary<Type, GlobalSetupManager>(); 

        public GlobalSetupManager GetSetupManager(Type type, Func<SpecificationFixture> fixtureFactory)
        {
            GlobalSetupManager result;
            if (_setupManagers.TryGetValue(type, out result))
                return result;

            result = new GlobalSetupManager(fixtureFactory);
            _setupManagers[type] = result;

            return result;
        }

        public void Dispose()
        {
            foreach (var globalSetupManager in _setupManagers.Values)
            {
                globalSetupManager.Close();
            }
            
            _setupManagers = new Dictionary<Type, GlobalSetupManager>();
        }
    }
}
