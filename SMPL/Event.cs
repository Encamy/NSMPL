using System;
using System.Collections.Generic;
using System.Text;

namespace SMPL
{
    public class Event
    {
        private UInt64 m_time;
        private UInt64 m_eventId;
        private UInt64 m_transactId;
        private ulong v;

        public Event(UInt64 eventId, UInt64 time, UInt64 transactId)
        {
            m_eventId = eventId;
            m_time = time;
            m_transactId = transactId;
        }

        public UInt64 Time { get => m_time; }
        public UInt64 EventId { get => m_eventId; }
        public UInt64 TransactId { get => m_transactId; }
    }
}
