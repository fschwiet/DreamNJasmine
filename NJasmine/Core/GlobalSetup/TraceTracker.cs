using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.Core.GlobalSetup
{
    public class TraceTracker
    {
        TraceEntry _top = null;

        public void AddTraceEntry(TestPosition position, string message)
        {
            _top = new TraceEntry()
            {
                Position = position,
                Message = message,
                Next = _top
            };
        }

        public void UnwindToPosition(TestPosition position)
        {
            while (_top != null)
            {
                if (!_top.Position.IsOnPathTo(position))
                    _top = _top.Next;
                else
                    break;
            }
        }

        public void UnwindAll()
        {
            _top = null;
        }

        class TraceEntry
        {
            public TestPosition Position;
            public string Message;
            public TraceEntry Next;
        }

        public IEnumerable<string> GetCurrentTraceMessages()
        {
            LinkedList<string> messages = new LinkedList<string>();

            var cursor = _top;

            while(cursor != null)
            {
                messages.AddLast(cursor.Message);
                cursor = cursor.Next;
            }

            return messages;
        }
    }
}
