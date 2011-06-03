using System;
using System.Linq;
using System.Threading;

namespace NJasmine.Core
{
    public class GlobalSetupManager : IGlobalSetupManager
    {
        Func<SpecificationFixture> _fixtureFactory;
        Thread _thread;
        GlobalSetupVisitor _visitor;
        AutoResetEvent _threadAtTargetPosition;
        AutoResetEvent _threadWaitingForTargetPosition;
        private TestPosition _targetPosition;

        public void Initialize(Func<SpecificationFixture> fixtureFactory)
        {
            _fixtureFactory = fixtureFactory;
            _thread = null;
            _threadAtTargetPosition = new AutoResetEvent(false);
            _threadWaitingForTargetPosition = new AutoResetEvent(false);
            _visitor = new GlobalSetupVisitor(_threadAtTargetPosition, _threadWaitingForTargetPosition);
        }

        public void Cleanup()
        {
            if (_thread != null)
            {
                PrepareForTestPosition(new TestPosition());
                _thread = null;
            }
        }

        public void PrepareForTestPosition(TestPosition position)
        {
            if (position == null)
                throw new ArgumentException("Parameter is required", "position");

            _targetPosition = position;
            _visitor.SetTargetPosition(position);

            if (_thread == null)
            {
                _thread = new Thread(ThreadProc);
                _thread.Start();
            }

            _threadWaitingForTargetPosition.Set();
            
            if (!_threadAtTargetPosition.WaitOne(-1))
            {
                throw new Exception("failed to prepare for a test in time");
            }
        }

        public T GetSetupResultAt<T>(TestPosition position)
        {
            return (T)_visitor.GetSetupResultAt(position);
        }

        public void ThreadProc()
        {
            while(true)
            {
                if (_targetPosition.Coordinates.Count() == 0)
                {
                    _threadAtTargetPosition.Set();
                    _threadWaitingForTargetPosition.WaitOne(-1);
                }

                var fixture = _fixtureFactory();
                fixture.CurrentPosition = new TestPosition(0);
                try
                {
                    fixture.Visitor = _visitor;
                    fixture.Run();
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