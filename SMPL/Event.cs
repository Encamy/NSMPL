using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SMPL
{
    public class Event : IComparable<Event>
    {
        private UInt64 m_time;
        private UInt64 m_eventId;
        private UInt64 m_transactId;

        public Event(UInt64 eventId, UInt64 time, UInt64 transactId)
        {
            m_eventId = eventId;
            m_time = time;
            m_transactId = transactId;
        }

        public UInt64 Time { get => m_time; }
        public UInt64 EventId { get => m_eventId; }
        public UInt64 TransactId { get => m_transactId; }

        public int CompareTo(Event other)
        {
            int result = this.Time.CompareTo(other.Time);
            if (result < 0)
            {
                return 1;
            }
            else if (result > 0)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
