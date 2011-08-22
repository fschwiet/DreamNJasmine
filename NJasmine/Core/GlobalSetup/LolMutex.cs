using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NJasmine.Core.GlobalSetup
{
    public class LolMutex
    {
        private readonly AutoResetEvent _resetEvent;

        public LolMutex(AutoResetEvent resetEvent)
        {
            _resetEvent = resetEvent;
        }

        public void PassAndWaitForTurn()
        {
            PassTurn();
            Thread.Sleep(0);
            WaitForTurn();
        }

        public void WaitForTurn()
        {
            _resetEvent.WaitOne(-1);
        }

        public void PassTurn()
        {
            _resetEvent.Set();
        }
    }
}
