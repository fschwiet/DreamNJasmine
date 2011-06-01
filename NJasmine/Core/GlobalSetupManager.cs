using System;
using System.Linq;
using System.Threading;

namespace NJasmine.Core
{
    public class GlobalSetupManager : IGlobalSetupManager
    {
        SpecificationFixture _fixture;
        Thread _thread;
        GlobalSetupVisitor _visitor;
        AutoResetEvent _threadReady;
        AutoResetEvent _threadWaiting;
        bool _exit;

        public void Initialize(SpecificationFixture fixture)
        {
            _fixture = fixture;
            _thread = null;
            _threadReady = new AutoResetEvent(false);
            _threadWaiting = new AutoResetEvent(false);
            _visitor = new GlobalSetupVisitor(_threadReady, _threadWaiting);
        }

        public void Cleanup()
        {
            if (_thread != null)
            {
                _exit = true;
                PrepareForTestPosition(new TestPosition());
                _thread = null;
            }
        }

        public void PrepareForTestPosition(TestPosition position)
        {
            _visitor.SetTargetPosition(position);

            if (_thread == null)
            {
                _thread = new Thread(ThreadProc);
                _thread.Start();
            }

            _threadWaiting.Set();
        }

        public T GetSetupResultAt<T>(TestPosition position)
        {
            return (T)_visitor.GetSetupResultAt(position);
        }

        public void ThreadProc()
        {
            while(!_exit)
            {
                _threadWaiting.WaitOne(-1);

                if (_exit)
                    break;

                try
                {
                    _fixture.Visitor = _visitor;
                    _fixture.Run();
                }
                catch (NJasmineTestMethod.TestFinishedException e)
                {
                }
                finally
                {
                    _visitor.FinishCleanup();
                }
            }
        }
    }
}